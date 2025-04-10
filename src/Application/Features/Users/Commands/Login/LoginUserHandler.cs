using System.IdentityModel.Tokens.Jwt;
using Application.Common.Errors;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Token;
using Application.Common.Interfaces.UnitOfWorks;
using Application.Features.Common.Mapping.Users;
using Contracts.ApiWrapper;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SharedKernel.Common.Messages;
using SharedKernel.Constants;
using SharedKernel.Extensions;
using Wangkanai.Detection.Services;

namespace Application.Features.Users.Commands.Login;

public class LoginUserHandler(
    IUnitOfWork unitOfWork,
    ITokenFactory tokenFactory,
    IDetectionService detectionService,
    ICurrentUser currentUser
) : IRequestHandler<LoginUserCommand, Result<LoginUserResponse>>
{
    public async ValueTask<Result<LoginUserResponse>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken
    )
    {
        User? user = await unitOfWork
            .DynamicReadOnlyRepository<User>()
            .FindByConditionAsync(
                new GetUserByUsernameSpecification(request.Username!),
                cancellationToken
            );
        if (user == null)
        {
            return Result<LoginUserResponse>.Failure(
                new NotFoundError(
                    "Resource is not found",
                    Messager.Create<User>().Message(MessageType.Found).Negative().BuildMessage()
                )
            );
        }
        if (!Verify(request.Password, user.Password))
        {
            return Result<LoginUserResponse>.Failure(
                new BadRequestError(
                    "Error has occured with password",
                    Messager
                        .Create<User>()
                        .Property(x => x.Password)
                        .Message(MessageType.Correct)
                        .Negative()
                        .BuildMessage()
                )
            );
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
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            ],
            refreshExpireTime
        );

        userToken.RefreshToken = refreshToken;

        await unitOfWork.Repository<UserToken>().AddAsync(userToken, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return Result<LoginUserResponse>.Success(
            new()
            {
                Token = accessToken,
                Refresh = refreshToken,
                AccessTokenExpiredIn = (long)
                    Math.Ceiling((accesstokenExpiredTime - DateTime.UtcNow).TotalSeconds),
                TokenType = JwtBearerDefaults.AuthenticationScheme,
                User = user.ToUserProjection(),
            }
        );
    }
}
