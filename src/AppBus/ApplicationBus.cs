using System;
using System.Collections.Generic;

namespace AppBus
{
    public interface IApplicationBus
    {
        void Send(object message);
        void Add(Type type);
    }

    public class ApplicationBus : List<Type>, IApplicationBus
    {
        private readonly IMessageHandlerFactory messageHandlerFactory;

        public ApplicationBus(IMessageHandlerFactory messageHandlerFactory)
        {
            this.messageHandlerFactory = messageHandlerFactory;
        }

        public new void Add(Type type)
        {
            if (TypeIsNotAMessageHandler(type))
                throw ExceptionForTypesThatAreNotMessageHandlers(type);
            base.Add(type);
        }

        private static InvalidOperationException ExceptionForTypesThatAreNotMessageHandlers(Type type)
        {
            return new InvalidOperationException(string.Format("Type {0} must implement the IMessageHandler interface", type.Name));
        }

        private static bool TypeIsNotAMessageHandler(Type type)
        {
            return type.GetInterface(typeof (IMessageHandler).Name) == null;
        }

        public IEnumerable<IMessageHandler> GetHandlersForType(Type type)
        {
            foreach (var handlerType in this)
            {
                var handler = messageHandlerFactory.Create(handlerType);
                if (handler.CanHandle(type))
                    yield return handler;
            }
        }

        public void Send(object message)
        {
            foreach (var handler in GetHandlersForType(message.GetType()))
                handler.Handle(message);
        }
    }
}