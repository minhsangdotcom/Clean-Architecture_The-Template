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

public class Message<T>(string? subjectName = null)
    where T : class
{
    private bool? isNegative = null;

    private string objectName = string.Empty;

    private string propertyName = string.Empty;

    private readonly string subjectName = string.IsNullOrWhiteSpace(subjectName)
        ? typeof(T).Name
        : subjectName;

    private string CustomMessage = string.Empty;

    private MessageType type = 0;

    private readonly Dictionary<MessageType, KeyValuePair<string, string?>> Messages =
        CommonMessage();

    public void SetNegative(bool value) => isNegative = value;

    public void SetObjectName(string value) => objectName = value;

    public void SetPropertyName(string value) => propertyName = value;

    public void SetCustomMessage(string value) => CustomMessage = value;

    public void SetMessage(MessageType value) => type = value;

    public string BuildMessage()
    {
        var messageBuilder = new StringBuilder(
            $"{subjectName}_{propertyName.ToScreamingSnakeCase()}"
        );

        if (isNegative == true && (type == 0 || string.IsNullOrWhiteSpace(Messages[type].Value)))
        {
            messageBuilder.Append($"_{"not".ToScreamingSnakeCase()}");
        }

        string message = CustomMessage.ToScreamingSnakeCase();

        if (string.IsNullOrWhiteSpace(message))
        {
            message =
                isNegative == true && !string.IsNullOrWhiteSpace(Messages[type].Value)
                    ? Messages[type].Value!
                    : Messages[type].Key;
        }

        messageBuilder.Append($"_{message}");

        if (!string.IsNullOrWhiteSpace(objectName))
        {
            messageBuilder.Append($"_{objectName}");
        }

        return messageBuilder.ToString().ToUpper();
    }

    private static Dictionary<MessageType, KeyValuePair<string, string?>> CommonMessage() =>
        new()
        {
            { MessageType.MaximumLength, new("TOO_LONG", "TOO_LONG") },
            { MessageType.MinumumLength, new("TOO_SHORT", "TOO_SHORT") },
            { MessageType.ValidFormat, new("VALID_FORMAT", "INVALID_FORMAT") },
            { MessageType.Found, new("FOUND", null) },
            { MessageType.Existence, new("EXISTENCE", null) },
            { MessageType.Correct, new("CORRECT", "INCORRECT") },
            { MessageType.Active, new("ACTIVE", "DEACTIVE") },
            { MessageType.OuttaOption, new("OUTTA_OPTIONS", null) },
            { MessageType.GreaterThan, new("GREATER_THAN", null) },
            { MessageType.GreaterThanEqual, new("GREATER_THAN_EQUAL", null) },
            { MessageType.LessThan, new("LESS_THAN", null) },
            { MessageType.LessThanEqual, new("LESS_THAN_EQUAL", null) },
            { MessageType.Null, new("NULL", null) },
            { MessageType.Empty, new("EMPTY", null) },
            { MessageType.Unique, new("UNIQUE", "NON_UNIQUE") },
            { MessageType.Strong, new("STRONG_ENOUGH", null) },
        };
}

public enum MessageType
{
    MaximumLength = 1,
    MinumumLength = 2,
    ValidFormat = 3,
    Found = 4,
    Existence = 5,
    Correct = 6,
    Active = 7,
    OuttaOption = 8,
    GreaterThan = 9,
    GreaterThanEqual = 10,
    LessThan = 11,
    LessThanEqual = 12,
    Empty = 13,
    Null = 14,
    Unique = 15,
    Strong = 16,
}
