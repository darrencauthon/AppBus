using System;

namespace AppBus
{
    public abstract class MessageHandler<T> : IMessageHandler
    {
        public bool CanHandle(Type type)
        {
            return type == typeof (T);
        }

        public void Handle(object message)
        {
            Handle((T)message);
        }

        public abstract void Handle(T message);
    }
}