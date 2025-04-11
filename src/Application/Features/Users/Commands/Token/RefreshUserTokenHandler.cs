using System.IdentityModel.Tokens.Jwt;
using Application.Common.Errors;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Token;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.ApiWrapper;
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
    ITokenFactoryService tokenFactory,
    IDetectionService detectionService,
    ICurrentUser currentUser
) : IRequestHandler<RefreshUserTokenCommand, Result<RefreshUserTokenResponse>>
{
    public async ValueTask<Result<RefreshUserTokenResponse>> Handle(
        RefreshUserTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        Result<DecodeTokenResponse> result = ValidateRefreshToken(command.RefreshToken!);
        DecodeTokenResponse decodeToken = result.Value!;

        IList<UserToken> refreshTokens = await unitOfWork
            .DynamicReadOnlyRepository<UserToken>()
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

        // detect cheating with token, maybe which is stolen
        if (validRefreshToken == null)
        {
            // remove all the token by family token
            await unitOfWork.Repository<UserToken>().DeleteRangeAsync(refreshTokens);
            await unitOfWork.SaveAsync(cancellationToken);

            return Result<RefreshUserTokenResponse>.Failure(
                new BadRequestError(
                    "Error has occured with the Refresh token",
                    Messager
                        .Create<UserToken>(nameof(User))
                        .Property(x => x.RefreshToken!)
                        .Negative()
                        .Message(MessageType.Matching)
                        .ObjectName("CurrentToken")
                        .BuildMessage()
                )
            );
        }

        if (validRefreshToken.User!.Status == UserStatus.Inactive)
        {
            return Result<RefreshUserTokenResponse>.Failure(
                new BadRequestError(
                    "Error has occured with the current user",
                    Messager.Create<User>().Message(MessageType.Active).Negative().BuildMessage()
                )
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

        return Result<RefreshUserTokenResponse>.Success(
            new() { Token = accessToken, RefreshToken = refreshToken }
        );
    }

    private Result<DecodeTokenResponse> ValidateRefreshToken(string token)
    {
        try
        {
            return Result<DecodeTokenResponse>.Success(tokenFactory.DecodeToken(token));
        }
        catch (Exception)
        {
            return Result<DecodeTokenResponse>.Failure(
                new BadRequestError(
                    "Error has occured with the Refresh token",
                    Messager
                        .Create<UserToken>(nameof(User))
                        .Property(x => x.RefreshToken!)
                        .Message(MessageType.Valid)
                        .Negative()
                        .BuildMessage()
                )
            );
        }
    }
}
