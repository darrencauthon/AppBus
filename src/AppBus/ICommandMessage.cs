namespace AppBus
{
    public interface ICommandMessage<TResult> : IEventMessage where TResult : ICommandResult
    {
        TResult Result { get; }
    }

    public interface ICommandResult
    {
        bool Success { get; set; }
    }
}