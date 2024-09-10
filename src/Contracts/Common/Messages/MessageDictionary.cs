using Ardalis.GuardClauses;
using Contracts.Extensions;

namespace Contracts.Common.Messages;

public class MessageDictionary(
    string message,
    string en,
    string vi,
    MessageType type,
    string? negativeMessage = null
)
{
    public string? Message { get; set; } = Guard.Against.NullOrEmpty(message, nameof(message));

    public string? En { get; set; } = Guard.Against.NullOrEmpty(en, nameof(en));

    public string? Vi { get; set; } = Guard.Against.NullOrEmpty(vi, nameof(vi));

    public string? NegativeMessage { get; set; } = negativeMessage?.ToKebabCase();

    public string? EnNegativeMessage { get; set; } = negativeMessage;

    public MessageType Type { get; set; } = type;
}