using Application.UseCases.Projections.Regions;
using Application.UseCases.Projections.Roles;

namespace Application.UseCases.Projections.Users;

public class UserDetailProjection : UserProjection
{
    public IEnumerable<RoleDetailProjection>? Roles { get; set; }

    public IEnumerable<UserClaimDetailProjection>? Claims { get; set; }

    public string? Street { get; set; }

    public ProvinceProjection? Province { get; set; }

    public DistrictProjection? District { get; set; }

    public CommuneProjection? Commune { get; set; }
}