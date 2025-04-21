using Application.Common.Errors;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.ApiWrapper;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Mediator;
using SharedKernel.Common.Messages;

namespace Application.Features.Users.Commands.ChangePassword;

public class ChangeUserPasswordHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    : IRequestHandler<ChangeUserPasswordCommand, Result<string>>
{
    public async ValueTask<Result<string>> Handle(
        ChangeUserPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        Ulid? userId = currentUser.Id;

        User? user = await unitOfWork
            .DynamicReadOnlyRepository<User>()
            .FindByConditionAsync(
                new GetUserByIdWithoutIncludeSpecification(userId ?? Ulid.Empty),
                cancellationToken
            );

        if (user == null)
        {
            return Result<string>.Failure(
                new NotFoundError(
                    "The resource is not found",
                    Messager.Create<User>().Message(MessageType.Found).Negative().Build()
                )
            );
        }

        if (!Verify(request.OldPassword, user.Password))
        {
            return Result<string>.Failure(
                new BadRequestError(
                    "Error has occured with password",
                    Messager
                        .Create<ChangeUserPasswordCommand>(nameof(User))
                        .Property(x => x.OldPassword!)
                        .Message(MessageType.Correct)
                        .Negative()
                        .Build()
                )
            );
        }

        user.SetPassword(HashPassword(request.NewPassword));

        await unitOfWork.Repository<User>().UpdateAsync(user);
        await unitOfWork.SaveAsync(cancellationToken);

        return Result<string>.Success();
    }
}
