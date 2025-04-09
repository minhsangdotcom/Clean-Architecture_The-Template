using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Errors;

public class BadRequestError(List<InvalidParam> invalidParams)
    : ErrorDetails(
        "The request parameters didn't validate.",
        invalidParams,
        nameof(ValidationError),
        StatusCodes.Status400BadRequest
    );
