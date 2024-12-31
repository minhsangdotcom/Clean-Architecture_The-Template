namespace Application.Common.Auth;

public class AuthorizeModel
{
    public List<string>? Roles { get; set; }

    public List<string>? Permissions { get; set; }
}
