using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBus
{
    public interface IApplicationBus
    {
        void Send<T>(T message);
        void Add(Type messageHandlerType);
    }

    public class ApplicationBus : List<ApplicationBusRegistration>, IApplicationBus
    {
        private readonly IMessageHandlerFactory messageHandlerFactory;

        public ApplicationBus(IMessageHandlerFactory messageHandlerFactory)
        {
            this.messageHandlerFactory = messageHandlerFactory;
        }

        public void Send<T>(T message)
        {
            foreach (var type in GetTypesOfMessageHandlers<T>())
                HandleTheMessage(type, message);
        }

        public void Add(Type messageHandlerType)
        {
            var messageType = GetTheMessageType(messageHandlerType);

            if (TheMessageTypeImplementsIEventMessage(messageType))
                Add(new ApplicationBusRegistration{
                                                      MessageHandlerType = messageHandlerType,
                                                      MessageType = messageType,
                                                  });
        }

        private void HandleTheMessage<T>(Type type, T message)
        {
            var handler = CreateMessageHandler<T>(type);
            handler.Handle(message);
        }

        private IMessageHandler<T> CreateMessageHandler<T>(Type type)
        {
            return messageHandlerFactory.Create<T>(type);
        }

        private IEnumerable<Type> GetTypesOfMessageHandlers<T>()
        {
            return GetRegistrationsForThisType<T>()
                .Select(x => x.MessageHandlerType);
        }

        private IEnumerable<ApplicationBusRegistration> GetRegistrationsForThisType<T>()
        {
            return this.Where(x => x.MessageType == typeof (T));
        }

        private static bool TheMessageTypeImplementsIEventMessage(Type messageType)
        {
            return messageType.GetInterfaces()
                .Any(x => x == typeof (IEventMessage));
        }

        private static Type GetTheMessageType(Type messageHandlerType)
        {
            return messageHandlerType.GetInterfaces()
                .Where(InheritsIMessageHandler()).First()
                .GetGenericArguments()[0];
        }

        private static Func<Type, bool> InheritsIMessageHandler()
        {
            return x => x.IsGenericType && x.FullName.StartsWith("AppBus.IMessageHandler`1");
        }
    }

    public class ApplicationBusRegistration
    {
        public Type MessageType { get; set; }
        public Type MessageHandlerType { get; set; }
    }
}