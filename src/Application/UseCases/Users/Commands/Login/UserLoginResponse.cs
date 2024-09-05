using Application.UseCases.Projections.Users;

namespace Application.UseCases.Users.Commands.Login;

public class UserLoginResponse : UserTokenProjection
{
    public UserProjection? User { get; set; }
}