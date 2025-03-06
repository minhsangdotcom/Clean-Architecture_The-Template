using System.Text.Json.Serialization;
using Domain.Aggregates.Users.Enums;
using SharedKernel.Models;

namespace Application.Features.Common.Projections.Users;

public class UserClaimDetailProjection : DefaultBaseResponse
{
    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public KindaUserClaimType Type { get; set; }
}
