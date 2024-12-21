using Application.UseCases.Projections.Users;
using Mediator;

namespace Application.UseCases.Users.Commands.Profiles;

public class UpdateUserProfileCommand : UserModel, IRequest<UpdateUserProfileResponse>;
