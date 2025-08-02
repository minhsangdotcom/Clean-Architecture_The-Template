namespace Application.Features.Permissions;

public class ListPermissionResponse : PermissionResponse
{
    public IReadOnlyCollection<PermissionResponse>? Children { get; set; }
}

public class PermissionResponse
{
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
}
