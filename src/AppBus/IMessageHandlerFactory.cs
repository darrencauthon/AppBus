using System;

namespace AppBus
{
    public interface IMessageHandlerFactory
    {
        IMessageHandler Create(Type type);
    }
}