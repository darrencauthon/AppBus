using System;
using System.Collections.Generic;

namespace AppBus
{
    public class ApplicationBus : List<Type>
    {
        private readonly IMessageHandlerFactory messageHandlerFactory;

        public ApplicationBus(IMessageHandlerFactory messageHandlerFactory)
        {
            this.messageHandlerFactory = messageHandlerFactory;
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
    }
}