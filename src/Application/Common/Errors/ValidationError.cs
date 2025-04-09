using Microsoft.AspNetCore.Http;

namespace Application.Common.Errors;

public class ValidationError(string title, string message)
    : ErrorDetails(title, message, nameof(BadRequestError), StatusCodes.Status400BadRequest);
