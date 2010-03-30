using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBus
{
    public class ApplicationBus: List<Type>
    {
        private readonly IMessageHandlerFactory messageHandlerFactory;

        public ApplicationBus(IMessageHandlerFactory messageHandlerFactory)
        {
            this.messageHandlerFactory = messageHandlerFactory;
        }

        public IEnumerable<IMessageHandler> GetHandlersForType(Type type)
        {
            var handlers = from item in this
                             select messageHandlerFactory.Create(item);

            return handlers.Where(x => x.CanHandle(type));
        }
    }
}