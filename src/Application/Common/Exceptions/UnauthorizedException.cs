using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public class UnauthorizedError(string title)
    : ErrorDetails(
        title,
        "You need to log in first to access this resource",
        nameof(UnauthorizedError),
        StatusCodes.Status401Unauthorized
    );
