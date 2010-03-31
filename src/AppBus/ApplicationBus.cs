using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBus
{
    public interface IApplicationBus
    {
        void Send<T>(T message);
        void Add<TMessage>(Type messageHandlerType);
    }

    public class ApplicationBus : List<ApplicationBusRegistration>, IApplicationBus
    {
        private readonly IMessageHandlerFactory messageHandlerFactory;

        public ApplicationBus(IMessageHandlerFactory messageHandlerFactory)
        {
            this.messageHandlerFactory = messageHandlerFactory;
        }

        public void Add<TMessage>(Type messageHandlerType)
        {
            var registration = new ApplicationBusRegistration{
                                                                 MessageHandlerType = messageHandlerType,
                                                                 MessageType = typeof (TMessage),
                                                             };
            Add(registration);
        }

        public void Send<T>(T message)
        {
            foreach (var type in GetTypesOfMessageHandlers<T>())
                HandleTheMessage(type, message);
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
    }

    public class ApplicationBusRegistration
    {
        public Type MessageType { get; set; }
        public Type MessageHandlerType { get; set; }
    }
}