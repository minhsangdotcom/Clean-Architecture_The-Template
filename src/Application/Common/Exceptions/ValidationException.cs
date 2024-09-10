using Contracts.ApiWrapper;
using Domain.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public class ValidationException : CustomException
{
    public int HttpStatusCode { get; private set; } = StatusCodes.Status400BadRequest;

    public IEnumerable<BadRequestError> ValidationErrors { get;}

    public ValidationException(IEnumerable<ValidationFailure> failures)
    {
        ValidationErrors = failures.GroupBy(x => x.PropertyName)
            .Select(failureGroups => new BadRequestError
            {
                PropertyName = failureGroups.Key,
                Reasons = failureGroups.Select(failure => new ReasonTranslation()
                {
                    Message = failure.ErrorMessage,
                }),
            });
    }
}