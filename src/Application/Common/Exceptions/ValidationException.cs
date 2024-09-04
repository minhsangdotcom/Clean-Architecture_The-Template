using FluentValidation.Results;
using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Http;

namespace Domain.Exceptions;

public class ValidationException : CustomException
{
    public int HttpStatusCode { get; private set; } = StatusCodes.Status400BadRequest;

    public IEnumerable<ValidationError> ValidationErrors { get;}

     public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        ValidationErrors = [];
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        ValidationErrors = failures.GroupBy(x => x.PropertyName)
            .Select(x => new ValidationError
            {
                Property = x.Key,
                Reasons = x.Select(x => x.ErrorMessage).ToList(),
            }).ToList();
    }
}