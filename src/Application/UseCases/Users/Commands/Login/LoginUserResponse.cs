using Application.UseCases.Projections.Users;

namespace Application.UseCases.Users.Commands.Login;

public class LoginUserResponse : UserTokenProjection
{
    public UserProjection? User { get; set; }
}