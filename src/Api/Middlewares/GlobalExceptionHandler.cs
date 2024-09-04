using System.Reflection;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class GlobalExceptionHandler(ILogger<InternalServerExceptionHandler> logger, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception, logger);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<InternalServerExceptionHandler> logger)
    {
        var types = Assembly.GetAssembly(typeof(IHandlerException<>))?.GetTypes()
            .Where(x => x.GetInterfaces().Any(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IHandlerException<>)))
            .Select(
                type =>
                    new
                    {
                        type,
                        iType = type.GetInterfaces().FirstOrDefault(
                                p =>
                                    p.GetGenericTypeDefinition() == typeof(IHandlerException<>)
                            )!.GenericTypeArguments[0],
                    }
            );

        Type? type = types?.FirstOrDefault(x => x.iType == exception.GetType())?.type;
        type ??= typeof(InternalServerExceptionHandler);

        await Invoke(type, context, exception, logger);
    }

    private async Task Invoke(Type type, HttpContext context, Exception exception, ILogger<InternalServerExceptionHandler> logger)
    {
        MethodInfo? method = type!.GetMethod(nameof(IHandlerException.Handle));

        List<object> param = [];

        if (type == typeof(InternalServerExceptionHandler))
        {
            param.Add(logger);
        }

        var handler = Activator.CreateInstance(type, [.. param]);

        await (Task)method?.Invoke(handler, [context, exception])!;
    }
}