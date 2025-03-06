using Contracts.Constants;

namespace Api.common.Routers;

public static class Router
{
    public static class UserRoute
    {
        public const string Users = $"{RoutePath.prefix}{nameof(Users)}";
        public const string GetUpdateDelete =
            $"{RoutePath.prefix}{nameof(Users)}/" + "{" + RoutePath.Id + "}";
        public const string GetRouteName = $"{Users}DetailEndpoint";

        public const string Profile = $"{RoutePath.prefix}{nameof(Users)}/{nameof(Profile)}";

        public const string ChangePassword =
            $"{RoutePath.prefix}{nameof(Users)}/{nameof(ChangePassword)}";
        public const string RequestResetPassowrd =
            $"{RoutePath.prefix}{nameof(Users)}/{nameof(RequestResetPassowrd)}";
        public const string ResetPassowrd =
            $"{RoutePath.prefix}{nameof(Users)}/{nameof(ResetPassowrd)}";

        public const string Login = $"{RoutePath.prefix}{nameof(Users)}/{nameof(Login)}";
        public const string RefreshToken =
            $"{RoutePath.prefix}{nameof(Users)}/{nameof(RefreshToken)}";
        public const string Tags = $"{nameof(Users)} endpoint";
    }

    public static class RoleRoute
    {
        public const string Roles = $"{RoutePath.prefix}{nameof(Roles)}";

        public const string GetUpdateDelete =
            $"{RoutePath.prefix}{nameof(Roles)}/" + "{" + RoutePath.Id + "}";

        public const string GetRouteName = $"{Roles}DetailEndpoint";

        public const string Tags = $"{nameof(Roles)} endpoint";
    }

    public static class PermissionRoute
    {
        public const string Permissions = $"{RoutePath.prefix}{nameof(Permissions)}";

        public const string Tags = $"{nameof(Permissions)} endpoint";
    }

    public static class AuditLogRoute
    {
        public const string AuditLog = $"{RoutePath.prefix}{nameof(AuditLog)}";
        public const string Tags = $"{nameof(AuditLog)} endpoint";
    }

    public static class RegionRoute
    {
        public const string Provinces = $"{RoutePath.prefix}{nameof(Provinces)}";
        public const string Districts = $"{RoutePath.prefix}{nameof(Districts)}";
        public const string Communes = $"{RoutePath.prefix}{nameof(Communes)}";
        public const string Tags = $"{nameof(RegionRoute)} endpoint";
    }
}
