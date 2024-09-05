using Application.UseCases.Projections.Users;
using Mediator;

namespace Application.UseCases.Users.Commands.Profiles;

public class UpdateUserProfileQuery : UserModel, IRequest<UpdateUserProfileResponse>;