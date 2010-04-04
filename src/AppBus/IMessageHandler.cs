namespace AppBus
{
    public interface IMessageHandler
    {
    }

    public interface IMessageHandler<T> : IMessageHandler
    {
        void Handle(T message);
    }
}