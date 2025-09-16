namespace ProperTea.ProperCqrs;

public interface ICommand;

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}

public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, object?>
    where TCommand : ICommand
{
}

public abstract class CommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand
{
    public abstract Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    async Task<object?> ICommandHandler<TCommand, object?>.HandleAsync(TCommand command, CancellationToken ct)
    {
        await HandleAsync(command, ct);
        return null;
    }

    public abstract Task HandleAsync(TCommand command, CancellationToken ct = default);
}

public interface ICommandBus
{
    Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand;
}