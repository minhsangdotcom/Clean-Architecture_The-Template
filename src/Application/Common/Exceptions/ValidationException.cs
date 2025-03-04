using Contracts.ApiWrapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using SharedKernel.Common.Messages;
using SharedKernel.Exceptions;

namespace Application.Common.Exceptions;

public class ValidationException() : CustomException("One or more validation errors have occured")
{
    public int HttpStatusCode { get; private set; } = StatusCodes.Status400BadRequest;

    public IEnumerable<BadRequestError> ValidationErrors { get; } = [];

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        ValidationErrors = failures
            .GroupBy(x => x.PropertyName)
            .Select(failureGroups => new BadRequestError
            {
                PropertyName = failureGroups.Key,
                Reasons = failureGroups.Select(failure =>
                {
                    MessageResult messageResult = (MessageResult)failure.CustomState;
                    return new ReasonTranslation()
                    {
                        Message = messageResult.Message,
                        En = messageResult.En,
                        Vi = messageResult.Vi,
                    };
                }),
            });
    }
}
