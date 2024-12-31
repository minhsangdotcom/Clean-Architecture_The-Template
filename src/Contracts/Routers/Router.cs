namespace Contracts.Routers;

public static class Router
{
    public const string Id = "id";
    public const string prefix = "api/";

    public static class UserRoute
    {
        public const string Users = $"{prefix}{nameof(Users)}";
        public const string GetUpdateDelete = $"{prefix}{nameof(Users)}/" + "{" + Id + "}";
        public const string GetRouteName = $"{Users}DetailEndpoint";

        public const string Profile = $"{prefix}{nameof(Users)}/{nameof(Profile)}";

        public const string ChangePassword = $"{prefix}{nameof(Users)}/{nameof(ChangePassword)}";
        public const string RequestResetPassowrd =
            $"{prefix}{nameof(Users)}/{nameof(RequestResetPassowrd)}";
        public const string ResetPassowrd = $"{prefix}{nameof(Users)}/{nameof(ResetPassowrd)}";

        public const string Login = $"{prefix}{nameof(Users)}/{nameof(Login)}";
        public const string RefreshToken = $"{prefix}{nameof(Users)}/{nameof(RefreshToken)}";
        public const string Tags = $"{nameof(Users)} endpoint";
    }

    public static class RoleRoute
    {
        public const string Roles = $"{prefix}{nameof(Roles)}";

        public const string GetUpdateDelete = $"{prefix}{nameof(Roles)}/" + "{" + Id + "}";

        public const string GetRouteName = $"{Roles}DetailEndpoint";

        public const string Tags = $"{nameof(Roles)} endpoint";
    }

    public static class PermissionRoute
    {
        public const string Permissions = $"{prefix}{nameof(Permissions)}";

        public const string Tags = $"{nameof(Permissions)} endpoint";
    }

    public static class AuditLogRoute
    {
        public const string AuditLog = $"{prefix}{nameof(AuditLog)}";
        public const string Tags = $"{nameof(AuditLog)} endpoint";
    }

    public static class RegionRoute
    {
        public const string Provinces = $"{prefix}{nameof(Provinces)}";
        public const string Districts = $"{prefix}{nameof(Districts)}";
        public const string Communes = $"{prefix}{nameof(Communes)}";
        public const string Tags = $"{nameof(RegionRoute)} endpoint";
    }

    public static class CartRoute
    {
        public const string Carts = $"{prefix}{nameof(Carts)}";
        public const string Pay = $"{prefix}{nameof(Carts)}/" + "{" + Id + "}" + $"/{nameof(Pay)}";
        public const string Tags = $"{nameof(CartRoute)} endpoint";
    }
}
