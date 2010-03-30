using System;

namespace AppBus
{
    public interface IMessageHandler
    {
        bool CanHandle(Type type);
    }
}