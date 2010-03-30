namespace AppBus
{
    public interface IQueryMessage<TResult> : IEventMessage
    {
        TResult Result { get; }
    }
}