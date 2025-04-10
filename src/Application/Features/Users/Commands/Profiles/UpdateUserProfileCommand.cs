using Application.Features.Common.Projections.Users;
using Contracts.ApiWrapper;
using Mediator;

namespace Application.Features.Users.Commands.Profiles;

public class UpdateUserProfileCommand : UserModel, IRequest<Result<UpdateUserProfileResponse>>;
