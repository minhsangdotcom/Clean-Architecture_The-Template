using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public class ForbiddenError(string title)
    : ErrorDetails(
        title,
        "You do not have enough permission to access this resource",
        nameof(ForbiddenError),
        StatusCodes.Status403Forbidden
    );
