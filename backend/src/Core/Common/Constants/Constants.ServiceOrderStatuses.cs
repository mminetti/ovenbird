namespace Core.Common.Constants;

public static partial class Constants
{
    public static class ServiceOrderStatuses
    {
        public const int New = 1;
        public const int InProgress = 2;
        public const int Scheduled = 3;
        public const int Cancelled = 4;
        public const int Rejected = 5;
        public const int Completed = 6;
        public const int Unexecuted = 7;
        public const int Error = 8;
    }
}
