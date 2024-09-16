using Mediator;

namespace Application.Common.Events;

public record EmailSenderEvent(string Email, string ResetLink, string? Template = null) : INotification;
