using Microsoft.AspNetCore.Http;
using SharedKernel.Exceptions;

namespace Application.Common.Exceptions;

public class ForbiddenException(string message) : CustomException(message)
{
    public int HttpStatusCode { get; private set; } = StatusCodes.Status403Forbidden;
}
