using Ardalis.GuardClauses;
using Contracts.Extensions;

namespace Contracts.Common.Messages;

public class MessageDictionary(
    string message,
    Dictionary<string, string> translation,
    MessageType type,
    string? negativeMessage = null
)
{
    /// <summary>
    /// Origin message
    /// </summary>
    public string? Message { get; set; } = Guard.Against.NullOrEmpty(message, nameof(message));

    public Dictionary<string, string> Translation { get; set; } =
        Guard.Against.Null(translation, nameof(translation));

    // public string? En { get; set; } = Guard.Against.NullOrEmpty(en, nameof(en));

    // public string? Vi { get; set; } = Guard.Against.NullOrEmpty(vi, nameof(vi));

    /// <summary>
    /// a better meanful of message in negative
    /// </summary>
    public string? NegativeMessage { get; set; } = negativeMessage?.ToKebabCase();

    public string? EnNegativeMessage { get; set; } = negativeMessage;

    public MessageType Type { get; set; } = type;
}
