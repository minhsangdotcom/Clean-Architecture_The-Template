using Microsoft.AspNetCore.Http;
using SharedKernel.Common.Messages;

namespace Application.Common.Exceptions;

public class NotFoundException(IEnumerable<MessageResult> errors) : BadRequestException(errors)
{
    public override int HttpStatusCode { get; protected set; } = StatusCodes.Status404NotFound;
}
