using System.IdentityModel.Tokens.Jwt;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Token;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using SharedKernel.Common.Messages;
using SharedKernel.Constants;
using SharedKernel.Models;
using Wangkanai.Detection.Services;

namespace Application.Features.Users.Commands.Token;

public class RefreshUserTokenHandler(
    IUnitOfWork unitOfWork,
    ITokenFactory tokenFactory,
    IDetectionService detectionService,
    ICurrentUser currentUser
) : IRequestHandler<RefreshUserTokenCommand, RefreshUserTokenResponse>
{
    public async ValueTask<RefreshUserTokenResponse> Handle(
        RefreshUserTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        DecodeTokenResponse decodeToken = ValidateRefreshToken(command.RefreshToken!);
        IList<UserToken> refreshTokens = await unitOfWork
            .ReadOnlyRepository<UserToken>()
            .ListAsync(
                new ListRefreshtokenByFamillyIdSpecification(
                    decodeToken.FamilyId!,
                    Ulid.Parse(decodeToken.Sub!)
                ),
                new()
                {
                    Sort =
                        $"{nameof(UserToken.CreatedAt).ToLower()}${OrderTerm.DELIMITER}{OrderTerm.DESC}",
                },
                cancellationToken
            );
        UserToken validRefreshToken = refreshTokens[0];

        if (validRefreshToken == null)
        {
            await unitOfWork.Repository<UserToken>().DeleteRangeAsync(refreshTokens);
            await unitOfWork.SaveAsync(cancellationToken);
            throw new BadRequestException(
                [
                    Messager
                        .Create<UserToken>(nameof(User))
                        .Property(x => x.RefreshToken!)
                        .Message(MessageType.Valid)
                        .Negative()
                        .BuildMessage(),
                ]
            );
        }

        if (validRefreshToken.User!.Status == UserStatus.Inactive)
        {
            throw new BadRequestException(
                [Messager.Create<User>().Message(MessageType.Active).Negative().BuildMessage()]
            );
        }

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

        await unitOfWork.Repository<UserToken>().AddAsync(userToken, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return new() { Token = accessToken, RefreshToken = refreshToken };
    }

    private DecodeTokenResponse ValidateRefreshToken(string token)
    {
        try
        {
            return tokenFactory.DecodeToken(token);
        }
        catch (Exception)
        {
            throw new BadRequestException(
                [
                    Messager
                        .Create<UserToken>(nameof(User))
                        .Property(x => x.RefreshToken!)
                        .Message(MessageType.Valid)
                        .Negative()
                        .BuildMessage(),
                ]
            );
        }
    }
}
