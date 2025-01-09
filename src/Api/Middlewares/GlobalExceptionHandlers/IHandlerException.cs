using Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Api.Middlewares.GlobalExceptionHandlers;

public interface IHandlerException<T> : IHandlerException
    where T : CustomException
{
}

public interface IHandlerException
{
    Task Handle(HttpContext httpContext, Exception ex);
}