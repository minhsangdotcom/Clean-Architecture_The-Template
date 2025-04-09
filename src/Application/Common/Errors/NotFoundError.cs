using Microsoft.AspNetCore.Http;

namespace Application.Common.Errors;

public class NotFoundError(string title, string message)
    : ErrorDetails(title, message, nameof(NotFoundError), StatusCodes.Status404NotFound);
