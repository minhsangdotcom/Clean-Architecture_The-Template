using System.Linq.Expressions;
using Contracts.Extensions.Expressions;

namespace Contracts.Common.Messages;

public static class Messager
{
    public static Message<T> Create<T>() where T : class => new();

    public static Message<T> Property<T>(this Message<T> message, Expression<Func<T,object>> prop) where T : class
    {
        message.SetPropertyName(prop.ToStringProperty());
        return message;
    }

    public static Message<T> Negative<T>(this Message<T> message, bool isNegative = true) where T : class
    {
        message.SetNegative(isNegative);
        return message;
    }

    public static Message<T> ObjectName<T>(this Message<T> message, string name) where T : class
    {
        message.SetObjectName(name);
        return message;
    }

    public static Message<T> Message<T>(this Message<T> message, string mes) where T : class
    {
        message.SetCustomMessage(mes);
        return message;
    }

    public static Message<T> Message<T>(this Message<T> message, MessageType mes) where T : class
    {
        message.SetMessage(mes);
        return message;
    }

    public static string Build<T>(this Message<T> message) where T : class
    {
        return message.BuildMessage();
    }
}