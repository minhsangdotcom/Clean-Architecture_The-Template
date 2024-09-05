namespace Application.UseCases.Users.Commands.Token;

public class UserTokenResponse
{
    public string? RefreshToken { get; set; }

    public string? Token { get; set; }
}