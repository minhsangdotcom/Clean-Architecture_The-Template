using Application.Features.Common.Projections.Users;
using Mediator;

namespace Application.Features.Users.Commands.Profiles;

public class UpdateUserProfileCommand : UserModel, IRequest<UpdateUserProfileResponse>;
