using System.Text;
using Contracts.Extensions;

namespace Contracts.Common.Messages;

public static class Message
{
    public const string SUCCESS = nameof(SUCCESS);

    public const string LOGIN_SUCCESS = $"LOGIN {nameof(SUCCESS)}";

    public const string UNAUTHORIZED = nameof(UNAUTHORIZED);

    public const string FORBIDDEN = nameof(FORBIDDEN);

    public const string TOKEN_EXPIRED = "TOKEN EXPIRED";
}

public class Message<T>(string? subjectName = null) where T : class
{
    private bool? isNegative = null;

    private string objectName = string.Empty;

    private string propertyName = string.Empty;

    private readonly string subjectName = string.IsNullOrWhiteSpace(subjectName) ? typeof(T).Name : subjectName;

    private string CustomMessage = string.Empty;

    private MessageType type = 0;

    private readonly Dictionary<MessageType, string> Messages = CommonMessage();

    public void SetNegative(bool value) => isNegative = value;

    public void SetObjectName(string value) => objectName = value;

    public void SetPropertyName(string value) => propertyName = value;

    public void SetCustomMessage(string value) => CustomMessage = value;

    public void SetMessage(MessageType value) => type = value;

    public string BuildMessage()
    {
        var messageBuilder = new StringBuilder($"{subjectName}_{propertyName.ToScreamingSnakeCase()}");

        if (isNegative != null && isNegative == false)
        {
            messageBuilder.Append($"_{"not".ToScreamingSnakeCase()}");
        }

        string message = string.IsNullOrWhiteSpace(CustomMessage) ? Messages[type] : CustomMessage.ToScreamingSnakeCase();

        messageBuilder.Append($"_{message}");

        if (!string.IsNullOrWhiteSpace(objectName))
        {
            messageBuilder.Append($"_{objectName}");
        }

        return messageBuilder.ToString().ToUpper();
    }

    private static Dictionary<MessageType, string> CommonMessage() => new()
    {
        {MessageType.MaximumLength, "TOO_LONG"},
        {MessageType.MinumumLength, "TOO_SHORT"},
        {MessageType.InvalidFormat, "INVALID_FORMAT"},
        {MessageType.Notfound, "NOT_FOUND"},
        {MessageType.Existence, "EXISTENCE"},
        {MessageType.NotCorrect, "NOT_CORRECT"},
        {MessageType.Deactive, "DEACTIVE"},
        {MessageType.OuttaOption, "OUTTA_OPTIONS"},
        {MessageType.GreaterThan, "GREATER_THAN"},
        {MessageType.GreaterThanEqual, "GREATER_THAN_EQUAL"},
        {MessageType.LessThan, "LESS_THAN"},
        {MessageType.LessThanEqual, "LESS_THAN_EQUAL"},
        {MessageType.Null, "NULL"},
        {MessageType.Empty, "EMPTY"},
        {MessageType.NonUnique, "NON_UNIQUE"},
    };
}

public enum MessageType
{
    MaximumLength = 1,
    MinumumLength = 2,
    InvalidFormat = 3,
    Notfound = 4,
    Existence = 5,
    NotCorrect = 6,
    Deactive = 7,
    OuttaOption = 8,
    GreaterThan = 9,
    GreaterThanEqual = 10,
    LessThan = 11,
    LessThanEqual = 12,
    Empty = 13,
    Null = 14,
    NonUnique = 15,
}