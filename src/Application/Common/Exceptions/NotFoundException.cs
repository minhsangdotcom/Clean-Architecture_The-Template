using Contracts.Common.Messages;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public class NotFoundException(IEnumerable<MessageResult> errors) : BadRequestException(errors)
{
    public override int HttpStatusCode { get; protected set; } = StatusCodes.Status404NotFound;
}