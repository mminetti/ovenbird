namespace UseCases.Common;

public class Constants
{
    public const int DEFAULT_PAGE_SIZE = 10;
    public const int MAX_PAGE_SIZE = 100;

    public static class CacheKeys
    {
        public const string CurrentUserPrefix = "current-user:";
        public const string SecurityTag = "security";
    }

    public static class Permissions
    {
        public const string UsersRead = "users.read";
        public const string UsersWrite = "users.write";
        public const string UsersDelete = "users.delete";
        public const string UsersManage = "users.manage";

        public const string RolesRead = "roles.read";
        public const string RolesWrite = "roles.write";
        public const string RolesDelete = "roles.delete";
        public const string RolesManage = "roles.manage";

        public const string PermissionsRead = "permissions.read";
        public const string PermissionsWrite = "permissions.write";
        public const string PermissionsDelete = "permissions.delete";

        public const string ModulesRead = "modules.read";
        public const string ModulesWrite = "modules.write";
        public const string ModulesDelete = "modules.delete";
    }
}
