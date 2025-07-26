using Application.Features.Common.Payloads.Users;
using Application.Features.Common.Projections.Users;
using Contracts.ApiWrapper;
using Mediator;

namespace Application.Features.Users.Commands.Profiles;

public class UpdateUserProfileCommand : UserPayload, IRequest<Result<UpdateUserProfileResponse>>;
