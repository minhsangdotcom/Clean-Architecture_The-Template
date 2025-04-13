using Contracts.Constants;

namespace Api.common.Routers;

public static class Router
{
    public static class UserRoute
    {
        public const string Users = nameof(Users);
        public const string GetUpdateDelete = $"{nameof(Users)}/" + "{" + RoutePath.Id + "}";
        public const string GetRouteName = $"{Users}DetailEndpoint";

        public const string Profile = $"{nameof(Users)}/{nameof(Profile)}";

        public const string ChangePassword = $"{nameof(Users)}/{nameof(ChangePassword)}";
        public const string RequestResetPassowrd =
            $"{nameof(Users)}/{nameof(RequestResetPassowrd)}";
        public const string ResetPassowrd = $"{nameof(Users)}/{nameof(ResetPassowrd)}";

        public const string Login = $"{nameof(Users)}/{nameof(Login)}";
        public const string RefreshToken = $"{nameof(Users)}/{nameof(RefreshToken)}";
        public const string Tags = $"{nameof(Users)} endpoint";
    }

    public static class RoleRoute
    {
        public const string Roles = nameof(Roles);

        public const string GetUpdateDelete = $"{nameof(Roles)}/" + "{" + RoutePath.Id + "}";

        public const string GetRouteName = $"{Roles}DetailEndpoint";

        public const string Tags = $"{nameof(Roles)} endpoint";
    }

    public static class PermissionRoute
    {
        public const string Permissions = nameof(Permissions);

        public const string Tags = $"{nameof(Permissions)} endpoint";
    }

    public static class AuditLogRoute
    {
        public const string AuditLog = nameof(AuditLog);
        public const string Tags = $"{nameof(AuditLog)} endpoint";
    }

    public static class RegionRoute
    {
        public const string Provinces = nameof(Provinces);
        public const string Districts = nameof(Districts);
        public const string Communes = nameof(Communes);
        public const string Tags = $"{nameof(RegionRoute)} endpoint";
    }
}
