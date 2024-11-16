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
    public string EntityName { get; } =
        string.IsNullOrWhiteSpace(entityName) ? typeof(T).Name : entityName;

    public string ObjectName { get; internal set; } = string.Empty;

    public string? PropertyName { get; internal set; } = string.Empty;

    public CustomMessage? CustomMessage { get; internal set; }

    public MessageType Type { get; internal set; } = 0;

    public bool? IsNegative { get; internal set; } = null;

    private readonly Dictionary<MessageType, MessageDictionary> Messages =
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
                MessageType.Valid,
                new(
                    MessageType.Valid.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "valid" },
                        { LanguageType.Vi.ToString(), "hợp lệ" },
                    },
                    MessageType.Valid,
                    "invalid",
                    new("for", "cho")
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
                MessageType.Missing,
                new(
                    MessageType.Missing.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "missing" },
                        { LanguageType.Vi.ToString(), "thiếu" },
                    },
                    MessageType.Missing
                )
            },
            {
                MessageType.Matching,
                new(
                    MessageType.Matching.ToString().ToKebabCase(),
                    new Dictionary<string, string>
                    {
                        { LanguageType.En.ToString(), "matching" },
                        { LanguageType.Vi.ToString(), "khớp" },
                    },
                    MessageType.Matching,
                    preposition: new("with", "với")
                )
            },
        };

    public MessageResult BuildMessage()
    {
        string subjectProperty = entityName.ToKebabCase();
        if (!string.IsNullOrWhiteSpace(PropertyName))
        {
            subjectProperty += $"_{PropertyName.ToKebabCase()}";
        }

        List<string> results = [subjectProperty];

        string? message = CustomMessage?.Message?.ToKebabCase() ?? Messages[Type].Message;
        string? negativeMessage = CustomMessage?.NegativeMessage ?? Messages[Type].NegativeMessage;
        string strMessage = BuildMainRawMessage(IsNegative, negativeMessage, message);
        results.Add(strMessage);

        if (!string.IsNullOrWhiteSpace(ObjectName))
        {
            results.Add(ObjectName.ToKebabCase());
        }

        string en = Translate(LanguageType.En);
        string vi = Translate(LanguageType.Vi);

        return new()
        {
            Message = string.Join("_", results).ToLower(),
            En = en,
            Vi = vi,
        };
    }

    private string Translate(LanguageType languageType)
    {
        string rootPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.FullName;
        string path = languageType switch
        {
            LanguageType.Vi => Path.Join(rootPath, "public", "Resources", "Message.vi.resx"),
            LanguageType.En => Path.Join(rootPath, "public", "Resources", "Message.en.resx"),
            _ => string.Empty,
        };

        Dictionary<string, ResourceResult> translator = ResourceExtension.ReadResxFile(path) ?? [];

        ResourceResult? propertyTranslation = translator!.GetValueOrDefault(PropertyName);
        string property = propertyTranslation?.Value ?? string.Empty;
        string entity = translator!.GetValueOrDefault(entityName)?.Value ?? string.Empty;
        string obj = translator.GetValueOrDefault(ObjectName)?.Value ?? string.Empty;

        MessageDictionary mess = Messages[Type];
        string message = BuildMainTranslationMessage(
            IsNegative,
            CustomMessage?.NegativeMessage ?? mess.NegativeMessage,
            CustomMessage?.CustomMessageTranslations[languageType.ToString()]
                ?? mess.Translation[languageType.ToString()],
            languageType
        );

        string messagePreposition = string.Empty;
        if (mess.Preposition.HasValue && !string.IsNullOrWhiteSpace(obj))
        {
            messagePreposition =
                languageType == LanguageType.En
                    ? mess.Preposition!.Value.Key
                    : mess.Preposition!.Value.Value;
        }

        string verb = string.Empty;
        if (languageType == LanguageType.En)
        {
            string[]? comment = propertyTranslation?.Comment?.Trim()?.Split(",");

            bool isPlural =
                comment != null
                && comment.Any(x =>
                {
                    string[] parts = x.Split("=");
                    return parts[0] == "IsPlural" && parts[1] == "true";
                });

            verb = isPlural ? "are" : "is";
        }

        string preposition = !string.IsNullOrWhiteSpace(property)
            ? (languageType == LanguageType.En ? "of" : "của")
            : string.Empty;

        List<string> words =
        [
            property,
            preposition,
            entity,
            verb,
            message,
            messagePreposition,
            obj,
        ];
        IReadOnlyCollection<string> notEmptyWord = words.FindAll(word =>
            !string.IsNullOrWhiteSpace(word)
        );
        return string.Join(' ', notEmptyWord);
    }

    private static string BuildMainRawMessage(
        bool? isNegative,
        string? negativeMessage,
        string message
    )
    {
        if (isNegative != true)
        {
            return message;
        }

        if (!string.IsNullOrWhiteSpace(negativeMessage))
        {
            return negativeMessage;
        }

        return string.Join('_', ["not", message]);
    }

    private static string BuildMainTranslationMessage(
        bool? isNegative,
        string? negativeMessage,
        string message,
        LanguageType languageType
    )
    {
        if (isNegative != true)
        {
            return message;
        }

        if (languageType == LanguageType.En && !string.IsNullOrWhiteSpace(negativeMessage))
        {
            return negativeMessage;
        }

        string localeNegativeWord = languageType == LanguageType.En ? "not" : "không";
        return $"{localeNegativeWord} {message}";
    }
}

public record CustomMessage(
    string Message,
    Dictionary<string, string> CustomMessageTranslations,
    string NegativeMessage
);
