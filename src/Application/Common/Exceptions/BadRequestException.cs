using Contracts.ApiWrapper;
using Contracts.Common.Messages;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public class BadRequestException(IEnumerable<MessageResult> errors) : CustomException("One or several errors have occured")
{
    public int HttpStatusCode { get; private set; } = StatusCodes.Status400BadRequest;

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
