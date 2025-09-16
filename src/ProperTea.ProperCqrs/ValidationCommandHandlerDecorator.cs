using FluentValidation;

namespace ProperTea.ProperCqrs;

public class ValidationCommandHandlerDecorator<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> decorated,
    IValidator<TCommand>? validator = null)
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand
{
    public async Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default)
    {
        if (validator == null)
            return await decorated.HandleAsync(command, ct);

        var validationResult = await validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        return await decorated.HandleAsync(command, ct);
    }
}

public class ValidationCommandHandlerDecorator<TCommand>(
    ICommandHandler<TCommand> decorated,
    IValidator<TCommand>? validator = null)
    :
        ValidationCommandHandlerDecorator<TCommand, object?>(decorated, validator), ICommandHandler<TCommand>
    where TCommand : ICommand;