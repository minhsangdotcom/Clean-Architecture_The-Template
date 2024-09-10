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

    private readonly Dictionary<MessageType, MessageDictionary> Messages = CommonMessage();

    public void SetNegative(bool value) => isNegative = value;

    public void SetObjectName(string value) => objectName = value;

    public void SetPropertyName(string value) => propertyName = value;

    public void SetCustomMessage(string value) => CustomMessage = value;

    public void SetMessage(MessageType value) => type = value;

    public MessageResult BuildMessage()
    {
        string subjectProperty = subjectName.ToKebabCase();

        if (!string.IsNullOrWhiteSpace(propertyName))
        {
            subjectProperty += $"_{propertyName.ToKebabCase()}";
        }

        var messageBuilder = new StringBuilder($"{subjectProperty}");

        if (
            isNegative == true
            && (type == 0 || string.IsNullOrWhiteSpace(Messages[type].NegativeMessage))
        )
        {
            messageBuilder.Append("_not");
        }

        string message = CustomMessage.ToKebabCase();

        if (string.IsNullOrWhiteSpace(message))
        {
            message =
                isNegative == true && !string.IsNullOrWhiteSpace(Messages[type].NegativeMessage)
                    ? Messages[type].NegativeMessage!
                    : Messages[type].Message!;
        }

        messageBuilder.Append($"_{message}");

        if (!string.IsNullOrWhiteSpace(objectName))
        {
            messageBuilder.Append($"_{objectName.ToKebabCase()}");
        }

        return new()
        {
            Message = messageBuilder.ToString().ToLower(),
            En = "",
            Vi = "",
        };
    }

    private static Dictionary<MessageType, MessageDictionary> CommonMessage() =>
        new()
        {
            {
                MessageType.MaximumLength,
                new("too-long", "too long", "quá dài", MessageType.MaximumLength)
            },
            {
                MessageType.MinumumLength,
                new("too-short", " too short", "quá ngắn", MessageType.MinumumLength)
            },
            {
                MessageType.ValidFormat,
                new(
                    MessageType.ValidFormat.ToString().ToKebabCase(),
                    "valid format",
                    "đúng định dạng",
                    MessageType.ValidFormat,
                    "invalid format"
                )
            },
            {
                MessageType.Found,
                new(
                    MessageType.Found.ToString().ToKebabCase(),
                    "found",
                    "tìm thấy",
                    MessageType.Found
                )
            },
            {
                MessageType.Existence,
                new(
                    MessageType.Existence.ToString().ToKebabCase(),
                    MessageType.Existence.ToString().ToKebabCase(),
                    "tồn tại",
                    MessageType.Existence
                )
            },
            {
                MessageType.Correct,
                new(
                    MessageType.Correct.ToString().ToKebabCase(),
                    MessageType.Correct.ToString().ToKebabCase(),
                    "đúng",
                    MessageType.Correct,
                    "incorrect"
                )
            },
            {
                MessageType.Active,
                new(
                    MessageType.Active.ToString().ToKebabCase(),
                    "active",
                    "hoạt động",
                    MessageType.Active,
                    "inactive"
                )
            },
            {
                MessageType.OuttaOption,
                new(
                    MessageType.OuttaOption.ToString().ToKebabCase(),
                    "outta options",
                    "hết tùy chọn",
                    MessageType.OuttaOption
                )
            },
            {
                MessageType.GreaterThan,
                new(
                    MessageType.GreaterThan.ToString().ToKebabCase(),
                    "greater than",
                    "lớn hơn",
                    MessageType.GreaterThan
                )
            },
            {
                MessageType.GreaterThanEqual,
                new(
                    MessageType.GreaterThanEqual.ToString().ToKebabCase(),
                    "greater than or equal",
                    "lớn hơn hoặc bằng",
                    MessageType.GreaterThanEqual
                )
            },
            {
                MessageType.LessThan,
                new(
                    MessageType.LessThan.ToString().ToKebabCase(),
                    "less than",
                    "nhỏ hơn",
                    MessageType.LessThan
                )
            },
            {
                MessageType.LessThanEqual,
                new(
                    MessageType.LessThanEqual.ToString().ToKebabCase(),
                    "less than or equal",
                    "nhỏ hơn hoặc bằng",
                    MessageType.LessThanEqual
                )
            },
            {
                MessageType.Null,
                new(MessageType.Null.ToString().ToKebabCase(), "null", "rỗng", MessageType.Null)
            },
            {
                MessageType.Empty,
                new(MessageType.Empty.ToString().ToKebabCase(), "empty", "trống", MessageType.Empty)
            },
            {
                MessageType.Unique,
                new(
                    MessageType.Unique.ToString().ToKebabCase(),
                    "unique",
                    "duy nhất",
                    MessageType.Unique,
                    "non-unique"
                )
            },
            {
                MessageType.Strong,
                new(
                    MessageType.Strong.ToString().ToKebabCase(),
                    "strong enough",
                    "đủ mạnh",
                    MessageType.Strong,
                    "weak"
                )
            },
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
