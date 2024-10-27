using System.IdentityModel.Tokens.Jwt;
using Application.Common.Exceptions;
using Application.Common.Interfaces.UnitOfWorks;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Token;
using Contracts.Common.Messages;
using Contracts.Constants;
using Contracts.Dtos.Models;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using Wangkanai.Detection.Services;

namespace Application.UseCases.Users.Commands.Token;

public class UserTokenHandler(
    IUnitOfWork unitOfWork,
    ITokenFactory tokenFactory,
    IDetectionService detectionService,
    ICurrentUser currentUser
) : IRequestHandler<UserTokenCommand, UserTokenResponse>
{
    public async ValueTask<UserTokenResponse> Handle(
        UserTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        DecodeTokenResponse decodeToken = tokenFactory.DecodeToken(command.RefreshToken!);

        UserToken? refresh = await unitOfWork
            .Repository<UserToken>()
            .FindByConditionAsync(
                new GetRefreshtokenSpecification(
                    command.RefreshToken!,
                    Ulid.Parse(decodeToken.Sub!)
                )
            );

        IEnumerable<UserToken> refreshTokens = await unitOfWork
            .Repository<UserToken>()
            .ListAsync(
                new ListRefreshtokenByFamillyIdSpecification(
                    decodeToken.FamilyId!,
                    Ulid.Parse(decodeToken.Sub!)
                ),
                new() { Sort = $"{nameof(UserToken.CreatedAt)} {OrderTerm.DESC}" }
            );

        if (refresh == null)
        {
            await unitOfWork.Repository<UserToken>().DeleteRangeAsync(refreshTokens);
            await unitOfWork.SaveAsync(cancellationToken);
            throw new BadRequestException(
                [
                    Messager
                        .Create<UserToken>(nameof(User))
                        .Property(x => x.RefreshToken!)
                        .Message(MessageType.Correct)
                        .Negative()
                        .BuildMessage(),
                ]
            );
        }

        if (refresh.User!.Status == UserStatus.Inactive)
        {
            throw new BadRequestException(
                [Messager.Create<User>().Message(MessageType.Active).Negative().BuildMessage()]
            );
        }

        await unitOfWork.Repository<UserToken>().DeleteRangeAsync(refreshTokens);

        var accesstokenExpiredTime = tokenFactory.AccesstokenExpiredTime;

        var accessToken = tokenFactory.CreateToken(
            [new(JwtRegisteredClaimNames.Sub.ToString(), decodeToken.Sub!.ToString())],
            accesstokenExpiredTime
        );

        var refreshTokenExpiredTime = tokenFactory.RefreshtokenExpiredTime;

        string refreshToken = tokenFactory.CreateToken(
            [
                new(JwtRegisteredClaimNames.Sub.ToString(), decodeToken.Sub!.ToString()),
                new(ClaimTypes.TokenFamilyId, decodeToken.FamilyId!),
                new(
                    JwtRegisteredClaimNames.UniqueName,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                ),
            ],
            refreshTokenExpiredTime
        );

        var userToken = new UserToken()
        {
            FamilyId = decodeToken.FamilyId,
            UserId = Ulid.Parse(decodeToken.Sub!),
            ExpiredTime = refreshTokenExpiredTime,
            RefreshToken = refreshToken,
            UserAgent = detectionService.UserAgent.ToString(),
            ClientIp = currentUser.ClientIp,
        };

        await unitOfWork.Repository<UserToken>().AddAsync(userToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return new UserTokenResponse() { Token = accessToken, RefreshToken = refreshToken };
    }
}
