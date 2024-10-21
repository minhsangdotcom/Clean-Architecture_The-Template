using Ardalis.GuardClauses;
using CaseConverter;

namespace Contracts.Common.Messages;

public class MessageDictionary(
    string message,
    Dictionary<string, string> translation,
    MessageType type,
    string? negativeMessage = null,
    KeyValuePair<string, string>? preposition = null
)
{
    /// <summary>
    /// Origin message
    /// </summary>
    public string? Message { get; set; } = Guard.Against.NullOrEmpty(message, nameof(message));

    public Dictionary<string, string> Translation { get; set; } =
        Guard.Against.Null(translation, nameof(translation));

    /// <summary>
    /// a meaningful negative message instead of using (not + message)
    /// </summary>
    public string? NegativeMessage { get; set; } = negativeMessage?.ToKebabCase();

    public string? EnNegativeMessage { get; set; } = negativeMessage;

    public MessageType Type { get; set; } = type;

    public KeyValuePair<string, string>? Preposition { get; set; } = preposition;
}
