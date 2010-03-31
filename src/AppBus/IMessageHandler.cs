namespace AppBus
{
    public interface IMessageHandler<T>
    {
        void Handle(T message);
    }
}