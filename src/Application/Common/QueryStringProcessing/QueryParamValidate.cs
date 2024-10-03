using Application.Common.Exceptions;
using Contracts.Common.Messages;
using Contracts.Dtos.Requests;

namespace Application.Common.QueryStringProcessing;

public static class QueryParamValidate
{
    public static QueryParamRequest ValidateQuery(this QueryParamRequest request)
    {
        if(!string.IsNullOrWhiteSpace(request.Cursor?.Before) && !string.IsNullOrWhiteSpace(request.Cursor?.After))
        {
            throw new BadRequestException(
                [
                    Messager.Create<QueryParamRequest>("QueryParam")
                        .Property(x => x.Cursor!)
                        .Message(MessageType.Redundant)
                        .Build()
                ]
            );
        }

        return request;
    }
}
