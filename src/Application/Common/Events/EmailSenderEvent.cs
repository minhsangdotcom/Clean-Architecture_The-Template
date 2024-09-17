using Mediator;

namespace Application.Common.Events;

public record EmailSenderEvent(
    string Email,
    string ResetLink,
    string Expiry,
    string? Template = null
) : INotification;
