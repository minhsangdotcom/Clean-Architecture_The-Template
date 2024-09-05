namespace Contracts.Routers;
public static class Router
{
    public const string Id = "id";

    public const string prefix = "api/";

    public static class PostRoute
    {
        public const string Posts = nameof(Posts);
        public const string GetPostById = $"{nameof(Posts)}/" + "{" + Id + "}";
        public const string Tags = $"{nameof(Posts)} endpoint";
    }

    public static class MediaRoute
    {
        public const string Medias = nameof(Medias);

        public const string GetMediaById = $"{nameof(Medias)}/" + "{" + Id + "}";

        public const string Tags = $"{nameof(Medias)} endpoint";
    }

    public static class UserRoute
    {
        public const string Users = $"{prefix}{nameof(Users)}";
        public const string GetUpdateDelete = $"{prefix}{nameof(Users)}/" + "{" + Id + "}";
        public const string GetRouteName = $"{Users}DetailEndpoint";

        public const string Profile = $"{prefix}{nameof(Users)}/{nameof(Profile)}";

        public const string ChangePassword = $"{prefix}{nameof(Users)}/{nameof(Profile)}/{nameof(ChangePassword)}";
        public const string Tags = $"{nameof(Users)} endpoint";
    }

    public static class AuthRoute
    {
        public const string Auths = $"{prefix}{nameof(Auths)}";

        /// <summary>
        /// Refresh token
        /// </summary>
        public const string Token = $"{prefix}{nameof(Token)}";

        public const string Tags = $"{nameof(Auths)} endpoint";
    }

    public static class RoleRoute
    {
        public const string Roles = $"{prefix}{nameof(Roles)}";

        public const string GetUpdateDelete = $"{prefix}{nameof(Roles)}/" + "{" + Id + "}";

        public const string GetRouteName = $"{Roles}DetailEndpoint";


        public const string Tags = $"{nameof(Roles)} endpoint";
    }
}