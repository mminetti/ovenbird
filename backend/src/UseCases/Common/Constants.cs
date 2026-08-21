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
}
