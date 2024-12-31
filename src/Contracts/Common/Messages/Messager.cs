using System.Linq.Expressions;
using Contracts.Extensions.Expressions;

namespace Contracts.Common.Messages;

public static class Messager
{
    public static Message<T> Create<T>(string? objectName = null)
        where T : class => new(objectName);

    public static Message<T> Property<T>(this Message<T> message, Expression<Func<T, object>> prop)
        where T : class
    {
        message.PropertyName = prop.ToStringProperty();
        return message;
    }

    public static Message<T> Property<T>(this Message<T> message, string propertyName)
        where T : class
    {
        message.PropertyName = propertyName;
        return message;
    }

    public static Message<T> Negative<T>(this Message<T> message, bool isNegative = true)
        where T : class
    {
        message.IsNegative = isNegative;
        return message;
    }

    public static Message<T> ObjectName<T>(this Message<T> message, string name)
        where T : class
    {
        message.ObjectName = name;
        return message;
    }

    public static Message<T> Message<T>(this Message<T> message, CustomMessage customMessage)
        where T : class
    {
        message.CustomMessage = customMessage;
        return message;
    }

    public static Message<T> Message<T>(this Message<T> message, MessageType mess)
        where T : class
    {
        message.Type = mess;
        return message;
    }

    public static MessageResult Build<T>(this Message<T> message)
        where T : class => message.BuildMessage();
}
