using System.Reflection;
using Api.Middlewares.GlobalExceptionHandlers;

namespace Api.Middlewares;

public class GlobalExceptionHandler(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception)
        {
            //await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var types = Assembly
            .GetAssembly(typeof(IHandlerException<>))
            ?.GetTypes()
            .Where(x =>
                x.GetInterfaces()
                    .Any(p =>
                        p.IsGenericType
                        && p.GetGenericTypeDefinition() == typeof(IHandlerException<>)
                    )
            )
            .Select(type => new
            {
                type,
                iType = type.GetInterfaces()
                    .FirstOrDefault(p =>
                        p.GetGenericTypeDefinition() == typeof(IHandlerException<>)
                    )!
                    .GenericTypeArguments[0],
            });

        Type? type = types?.FirstOrDefault(x => x.iType == exception.GetType())?.type;
        type ??= typeof(InternalServerExceptionHandler);

        await Invoke(type, context, exception);
    }

    private async Task Invoke(Type type, HttpContext context, Exception exception)
    {
        MethodInfo? method = type!.GetMethod(nameof(IHandlerException.Handle));

        var handler = Activator.CreateInstance(type);

        await (Task)method?.Invoke(handler, [context, exception])!;
    }
}
