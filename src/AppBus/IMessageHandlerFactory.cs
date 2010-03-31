using System;

namespace AppBus
{
    public interface IMessageHandlerFactory
    {
        IMessageHandler<T> Create<T>(Type type);
    }
}