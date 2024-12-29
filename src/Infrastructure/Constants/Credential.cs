using Contracts.Constants;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;

namespace Infrastructure.Constants;

public static class Credential
{
    public const string USER_DEFAULT_PASSWORD = "123456";

    public const string ADMIN_ROLE = "ADMIN";

    public const string MANAGER_ROLE = "MANAGER";

    public static readonly Dictionary<string, string[]> PermissionGroups =
        new()
        {
            {
                nameof(User) + "s",

                [
                    CreatePermission(ActionPermission.create, ObjectPermission.user),
                    CreatePermission(ActionPermission.update, ObjectPermission.user),
                    CreatePermission(ActionPermission.delete, ObjectPermission.user),
                    CreatePermission(ActionPermission.list, ObjectPermission.user),
                    CreatePermission(ActionPermission.detail, ObjectPermission.user),
                ]
            },
            {
                nameof(Role) + "s",

                [
                    CreatePermission(ActionPermission.create, ObjectPermission.role),
                    CreatePermission(ActionPermission.update, ObjectPermission.role),
                    CreatePermission(ActionPermission.delete, ObjectPermission.role),
                    CreatePermission(ActionPermission.list, ObjectPermission.role),
                    CreatePermission(ActionPermission.detail, ObjectPermission.role),
                ]
            },
        };

    public static readonly IReadOnlyCollection<KeyValuePair<string, string>> ADMIN_CLAIMS =
        PermissionGroups
            .SelectMany(x => x.Value)
            .Select(permission => new KeyValuePair<string, string>(
                ClaimTypes.Permission,
                permission
            ))
            .ToList();

    public static readonly IReadOnlyCollection<KeyValuePair<string, string>> MANAGER_CLAIMS =
    [
        new(ClaimTypes.Permission, $"{ActionPermission.create}:{ObjectPermission.user}"),
        new(ClaimTypes.Permission, $"{ActionPermission.list}:{ObjectPermission.user}"),
        new(ClaimTypes.Permission, $"{ActionPermission.detail}:{ObjectPermission.user}"),
        new(ClaimTypes.Permission, $"{ActionPermission.create}:{ObjectPermission.role}"),
        new(ClaimTypes.Permission, $"{ActionPermission.list}:{ObjectPermission.role}"),
        new(ClaimTypes.Permission, $"{ActionPermission.detail}:{ObjectPermission.role}"),
    ];

    public const string ADMIN_ROLE_ID = "01J79JQZRWAKCTCQV64VYKMZ56";
    public const string MANAGER_ROLE_ID = "01JB19HK30BGYJBZGNETQY8905";

    public static class UserIds
    {
        public static readonly Ulid CHLOE_KIM_ID = Ulid.Parse("01JD936AXSDNMQ713P5XMVRQDV");
        public static readonly Ulid JOHN_DOE_ID = Ulid.Parse("01JD936AXTYY9KABPPN4PGZP7N");
        public static readonly Ulid ALICE_SMITH_ID = Ulid.Parse("01JD936AXT7ECQRAREV6AAZZPM");
        public static readonly Ulid BOB_JOHNSON_ID = Ulid.Parse("01JD936AXTDYC4SPCNVCRHNS61");
        public static readonly Ulid EMILY_BROWN_ID = Ulid.Parse("01JD936AXT1YQDBJEXHPP9V8DA");
        public static readonly Ulid JAMES_WILLIAMS_ID = Ulid.Parse("01JD936AXT6X0CS4VZB7BK36BP");
        public static readonly Ulid OLIVIA_TAYLOR_ID = Ulid.Parse("01JD936AXVAGPSN007QAQGN00E");
        public static readonly Ulid DANIEL_LEE_ID = Ulid.Parse("01JD936AXVEJZM53ZHK13T6MFF");
        public static readonly Ulid SHOPHIA_GARCIA_ID = Ulid.Parse("01JD936AXVZBVDXQ6MQDZXHCS7");
        public static readonly Ulid MICHAEL_MARTINEZ_ID = Ulid.Parse("01JD936AXV6V9RTKSS4Z5Q773N");
        public static readonly Ulid ISABELLA_HARRIS_ID = Ulid.Parse("01JD936AXVVKTHBPDY8516E9M5");
        public static readonly Ulid DAVID_CLARK_ID = Ulid.Parse("01JD936AXVGEP5E7S0Z3VVM7VD");
        public static readonly Ulid EMMA_RODRIGUEZ_ID = Ulid.Parse("01JD936AXVEC1FGXZYHKA1Q7VG");
        public static readonly Ulid ANDREW_MOORE_ID = Ulid.Parse("01JD936AXVDZGQK7K1KNEB175H");
        public static readonly Ulid AVA_JACKSON_ID = Ulid.Parse("01JD936AXVJ3DJG8B1N17KFXF8");
        public static readonly Ulid JOSHUA_WHITE_ID = Ulid.Parse("01JD936AXWXDDYHNC7DFA9TB0R");
        public static readonly Ulid CHARLOTTE_THOMAS_ID = Ulid.Parse("01JD936AXWNKT0HR51PRNJ52W5");
        public static readonly Ulid ETHAN_KING_ID = Ulid.Parse("01JD936AXWFM847M47AZK1ARGV");
        public static readonly Ulid ABIGAIL_SCOTT_ID = Ulid.Parse("01JD936AXWJ9B8SEJC98P0P01P");
        public static readonly Ulid LIAM_PEREZ_ID = Ulid.Parse("01JD936AXWY0JMVNZW3KXXS5ZK");
    }

    public static string CreatePermission(string action, string obj) => $"{action}:{obj}";
}

public static class ActionPermission
{
    public const string create = nameof(create);
    public const string update = nameof(update);
    public const string delete = nameof(delete);
    public const string detail = nameof(detail);
    public const string list = nameof(list);
}

public static class ObjectPermission
{
    public const string user = nameof(user);
    public const string role = nameof(role);
}
