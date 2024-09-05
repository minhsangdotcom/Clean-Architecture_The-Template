using System.IdentityModel.Tokens.Jwt;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.UseCases.Projections.Users;
using AutoMapper;
using Contracts.Constants;
using Contracts.Extensions;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Wangkanai.Detection.Services;

namespace Application.UseCases.Users.Commands.Login;

public class LoginUserHandler(
    IUnitOfWork unitOfWork,
    ITokenFactory tokenFactory,
    IDetectionService detectionService,
    ICurrentUser currentUser,
    IMapper mapper
) : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    public async ValueTask<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        User user = await unitOfWork.Repository<User>().GetByConditionSpecificationAsync(
            new GetUserByUsernameSpecification(request.Username!)
        ) ?? throw new BadRequestException($"{nameof(User).ToUpper()}_NOTFOUND");

        if (!Verify(request.Password, user.Password))
        {
            throw new BadRequestException($"{nameof(User)}_PASSWORD_NOTCORRECT");
        }

        DateTime refreshExpireTime = tokenFactory.RefreshtokenExpiredTime;
        string familyId = StringExtension.GenerateRandomString(32);

        var userAgent = detectionService.UserAgent.ToString();

        var userToken = new UserToken()
        {
            ExpiredTime = refreshExpireTime,
            UserId = user.Id,
            FamilyId = familyId,
            UserAgent = userAgent,
            ClientIp = currentUser.ClientIp,
        };

        var accesstokenExpiredTime = tokenFactory.AccesstokenExpiredTime;

        string accessToken = tokenFactory.CreateToken(
            [new(JwtRegisteredClaimNames.Sub.ToString(), user.Id.ToString())],
            accesstokenExpiredTime
        );

        string refreshToken = tokenFactory.CreateToken(
            [
                new(ClaimTypes.TokenFamilyId, familyId),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            ],
            refreshExpireTime
        );

        userToken.RefreshToken = refreshToken;

        await unitOfWork.Repository<UserToken>().AddAsync(userToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return new LoginUserResponse()
        {
            Token = accessToken,
            Refresh = refreshToken,
            AccessTokenExpiredIn = (long)Math.Ceiling((accesstokenExpiredTime - DateTime.UtcNow).TotalSeconds),
            TokenType = JwtBearerDefaults.AuthenticationScheme,
            User = mapper.Map<UserProjection>(user),
        };
    }
}