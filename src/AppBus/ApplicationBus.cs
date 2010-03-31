using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBus
{
    public interface IApplicationBus
    {
        void Send(object message);
        void Add(Type type);
    }

    public class ApplicationBus : List<ApplicationBusRegistration>
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
            foreach (var type in this.Where(x => x.MessageType == typeof (T)))
                messageHandlerFactory.Create<T>(type.MessageHandlerType)
                    .Handle(message);
        }
    }

    public class ApplicationBusRegistration
    {
        public Type MessageType { get; set; }
        public Type MessageHandlerType { get; set; }
    }
}