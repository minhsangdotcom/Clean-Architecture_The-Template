using SharedKernel.Exceptions;

namespace Api.Middlewares.GlobalExceptionHandlers;

public interface IHandlerException<T> : IHandlerException
    where T : CustomException { }

public interface IHandlerException
{
    Task Handle(HttpContext httpContext, Exception ex);
}
