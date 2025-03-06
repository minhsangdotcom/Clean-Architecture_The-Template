using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Http;
using SharedKernel.Common.Messages;
using SharedKernel.Exceptions;

namespace Application.Common.Exceptions;

public class BadRequestException(IEnumerable<MessageResult> errors)
    : CustomException("One or several errors have occured")
{
    public virtual int HttpStatusCode { get; protected set; } = StatusCodes.Status400BadRequest;

    public IEnumerable<BadRequestError> Errors { get; set; } =
        errors.Select(x => new BadRequestError
        {
            Reasons =
            [
                new()
                {
                    Message = x.Message,
                    En = x.En,
                    Vi = x.Vi,
                },
            ],
        });
}
