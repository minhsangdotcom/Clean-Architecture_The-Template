using System.Text;
using CaseConverter;
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

public class Message<T>(string? entityName = null)
    where T : class
{
    private bool? isNegative = null;

    private string objectName = string.Empty;

    private string propertyName = string.Empty;

    private readonly string entityName = string.IsNullOrWhiteSpace(entityName)
        ? typeof(T).Name
        : entityName;

    private string CustomMessage = string.Empty;

    private Dictionary<string, string> CustomMessageTranslations = [];

    private MessageType type = 0;

    private readonly Dictionary<MessageType, MessageDictionary> Messages = CommonMessage();

    public void SetNegative(bool value) => isNegative = value;

    public void SetObjectName(string value) => objectName = value;

    public void SetPropertyName(string value) => propertyName = value;

    public void SetCustomMessage(string value) => CustomMessage = value;

    public void SetCustomMessageTranslations(Dictionary<string, string> translations) =>
        CustomMessageTranslations = translations;

    public void SetMessage(MessageType value) => type = value;

    public MessageResult BuildMessage()
    {
        string subjectProperty = entityName.ToKebabCase();

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
        string? en = CustomMessageTranslations[LanguageType.En.ToString()];
        string? vi = CustomMessageTranslations[LanguageType.En.ToString()];

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

        if (string.IsNullOrWhiteSpace(en) && string.IsNullOrWhiteSpace(vi))
        {
            en = Translation(LanguageType.En);
            vi = Translation(LanguageType.Vi);
        }

        return new()
        {
            Message = messageBuilder.ToString().ToLower(),
            En = en,
            Vi = vi,
        };
    }

    private string Translation(LanguageType languageType)
    {
        string rootPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.FullName;
        string path = languageType switch
        {
            LanguageType.Vi => Path.Join(rootPath, "public", "Resources", "Message.vi.resx"),

            LanguageType.En => Path.Join(rootPath, "public", "Resources", "Message.en.resx"),
            _ => string.Empty,
        };

        Dictionary<string, ResourceResult> translation = ResourceExtension.ReadResxFile(path);

        var propertyTranslation = translation.GetValueOrDefault(propertyName);

        string property = propertyTranslation?.Value ?? string.Empty;
        string entity = translation.GetValueOrDefault(entityName)?.Value ?? string.Empty;
        string obj = translation.GetValueOrDefault(objectName)?.Value ?? string.Empty;

        MessageDictionary mess = Messages[type];
        string message = mess.Translation[languageType.ToString()];

        string negative = string.Empty;

        if (
            isNegative == true
            && (
                string.IsNullOrWhiteSpace(mess.EnNegativeMessage) || languageType == LanguageType.Vi
            )
        )
        {
            negative = languageType == LanguageType.Vi ? "Không" : "not";
        }

        if (languageType == LanguageType.En && isNegative == true)
        {
            message = mess.EnNegativeMessage ?? mess.Message!;
        }

        string verb = string.Empty;

        if (languageType == LanguageType.En)
        {
            var comment = propertyTranslation?.Comment?.Trim()?.Split(",");

            bool isPlural =
                comment != null
                && comment.Any(x =>
                {
                    var c = x.Split("=");

                    return c[0] == "IsPlural" && c[1] == "true";
                });

            verb = isPlural ? "are" : "is";
        }

        string prePosition = !string.IsNullOrWhiteSpace(property)
            ? (languageType == LanguageType.En ? "of" : "của")
            : string.Empty;

        IEnumerable<string> results = [property, prePosition, entity, verb, negative, message, obj];

        return string.Join(" ", results.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private static Dictionary<MessageType, MessageDictionary> CommonMessage() =>
        new()
        {
            {
                MessageType.MaximumLength,
                new(
                    "too-long",
                    new Dictionary<string, string>()
                    {
                        { LanguageType.En.ToString(), "too long" },
                        { LanguageType.Vi.ToString(), "quá dài" },
                    },
                    MessageType.MaximumLength
                )
            },
            {
                MessageType.MinumumLength,
                new(
                    "too-short",
                    new Dictionary<string, string>()
                    {
                        { LanguageType.En.ToString(), "too short" },
                        { LanguageType.Vi.ToString(), "quá ngắn" },
                    },
                    MessageType.MinumumLength
                )
            },
            {
                MessageType.ValidFormat,
                new(
                    MessageType.ValidFormat.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "valid format" },
                        { LanguageType.Vi.ToString(), "đúng định dạng" },
                    },
                    MessageType.ValidFormat,
                    "invalid format"
                )
            },
            {
                MessageType.Found,
                new(
                    MessageType.Found.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "found" },
                        { LanguageType.Vi.ToString(), "tìm thấy" },
                    },
                    MessageType.Found
                )
            },
            {
                MessageType.Existence,
                new(
                    MessageType.Existence.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        {
                            LanguageType.En.ToString(),
                            MessageType.Existence.ToString().ToKebabCase()
                        },
                        { LanguageType.Vi.ToString(), "tồn tại" },
                    },
                    MessageType.Existence
                )
            },
            {
                MessageType.Correct,
                new(
                    MessageType.Correct.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        {
                            LanguageType.En.ToString(),
                            MessageType.Correct.ToString().ToKebabCase()
                        },
                        { LanguageType.Vi.ToString(), "đúng" },
                    },
                    MessageType.Correct,
                    "incorrect"
                )
            },
            {
                MessageType.Active,
                new(
                    MessageType.Active.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "active" },
                        { LanguageType.Vi.ToString(), "hoạt động" },
                    },
                    MessageType.Active,
                    "inactive"
                )
            },
            {
                MessageType.OuttaOption,
                new(
                    MessageType.OuttaOption.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "outta options" },
                        { LanguageType.Vi.ToString(), "hết tùy chọn" },
                    },
                    MessageType.OuttaOption
                )
            },
            {
                MessageType.GreaterThan,
                new(
                    MessageType.GreaterThan.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "greater than" },
                        { LanguageType.Vi.ToString(), "lớn hơn" },
                    },
                    MessageType.GreaterThan
                )
            },
            {
                MessageType.GreaterThanEqual,
                new(
                    MessageType.GreaterThanEqual.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "greater than or equal" },
                        { LanguageType.Vi.ToString(), "lớn hơn hoặc bằng" },
                    },
                    MessageType.GreaterThanEqual
                )
            },
            {
                MessageType.LessThan,
                new(
                    MessageType.LessThan.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "less than" },
                        { LanguageType.Vi.ToString(), "nhỏ hơn" },
                    },
                    MessageType.LessThan
                )
            },
            {
                MessageType.LessThanEqual,
                new(
                    MessageType.LessThanEqual.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "less than or equal" },
                        { LanguageType.Vi.ToString(), "nhỏ hơn hoặc bằng" },
                    },
                    MessageType.LessThanEqual
                )
            },
            {
                MessageType.Null,
                new(
                    MessageType.Null.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "null" },
                        { LanguageType.Vi.ToString(), "rỗng" },
                    },
                    MessageType.Null
                )
            },
            {
                MessageType.Empty,
                new(
                    MessageType.Empty.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "empty" },
                        { LanguageType.Vi.ToString(), "trống" },
                    },
                    MessageType.Empty
                )
            },
            {
                MessageType.Unique,
                new(
                    MessageType.Unique.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "unique" },
                        { LanguageType.Vi.ToString(), "là duy nhất" },
                    },
                    MessageType.Unique
                )
            },
            {
                MessageType.Strong,
                new(
                    MessageType.Strong.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "strong enough" },
                        { LanguageType.Vi.ToString(), "đủ mạnh" },
                    },
                    MessageType.Strong,
                    "weak"
                )
            },
            {
                MessageType.Expired,
                new(
                    MessageType.Expired.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "Expired" },
                        { LanguageType.Vi.ToString(), "Quá hạn" },
                    },
                    MessageType.Expired
                )
            },
            {
                MessageType.Redundant,
                new(
                    MessageType.Redundant.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "Redundant" },
                        { LanguageType.Vi.ToString(), "Dư thừa" },
                    },
                    MessageType.Redundant
                )
            },
            {
                MessageType.LackOfArrayOperatorIndex,
                new(
                    MessageType.LackOfArrayOperatorIndex.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "Lack of array operator index" },
                        { LanguageType.Vi.ToString(), "Thiếu chỉ số của toán tử mảng" },
                    },
                    MessageType.LackOfArrayOperatorIndex
                )
            },
            {
                MessageType.LackOfOperator,
                new(
                    MessageType.LackOfOperator.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "Lack of operator" },
                        { LanguageType.Vi.ToString(), "Thiếu toán tử" },
                    },
                    MessageType.LackOfOperator
                )
            },
            {
                MessageType.LackOfArrayOperatorElement,
                new(
                    MessageType.LackOfArrayOperatorElement.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "Lack of array operator element" },
                        { LanguageType.Vi.ToString(), "Thiếu phần tử của toán tử mảng" },
                    },
                    MessageType.LackOfArrayOperatorElement
                )
            },
            {
                MessageType.MustBeInteger,
                new(
                    MessageType.MustBeInteger.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "Must be integer" },
                        { LanguageType.Vi.ToString(), "Phải là số" },
                    },
                    MessageType.MustBeInteger
                )
            },
            {
                MessageType.MustBeDatetime,
                new(
                    MessageType.MustBeDatetime.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "Must be datetime" },
                        { LanguageType.Vi.ToString(), "Phải là kiểu ngày giờ" },
                    },
                    MessageType.MustBeDatetime
                )
            },
            {
                MessageType.MustBeUlid,
                new(
                    MessageType.MustBeUlid.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "Must be Ulid" },
                        { LanguageType.Vi.ToString(), "Phải là kiểu Ulid" },
                    },
                    MessageType.MustBeUlid
                )
            },
            {
                MessageType.LackOfProperty,
                new(
                    MessageType.LackOfProperty.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "Lack of property" },
                        { LanguageType.Vi.ToString(), "Thiếu thuộc tính" },
                    },
                    MessageType.LackOfProperty
                )
            },
        };
}
