using FluentValidation;
using Mediator;
using ValidationException = Domain.Exceptions.ValidationException;

namespace Application.Common.Behaviors;
public sealed class ValidationBehavior<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators) : MessagePreProcessor<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    protected override async ValueTask Handle(TMessage message, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TMessage>(message);

            var validationResults = await Task.WhenAll(
                validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Count != 0)
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return;
    }
}