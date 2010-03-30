using System;

namespace AppBus
{
    public abstract class MessageHandler<T> : IMessageHandler
    {
        public bool CanHandle(Type type)
        {
            return type == typeof (T);
        }

        public abstract void Handle(object message);
    }
}