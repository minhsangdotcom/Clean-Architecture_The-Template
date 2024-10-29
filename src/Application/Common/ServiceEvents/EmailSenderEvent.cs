using Mediator;

namespace Application.Common.ServiceEvents;

public record EmailSenderEvent(
    string Email,
    string ResetLink,
    string Expiry,
    string? Template = null
) : INotification;
