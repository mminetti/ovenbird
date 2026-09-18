namespace Core.Common;

public static partial class Constants
{
    public static class ConnectorImplementations
    {
        public const int FluentFtpService = 1;
        public const int SshNetSftpService = 2;
        public const int LocalFileSystemFtpService = 3;
        public const int AzureBlobFileStorage = 4;
        public const int LocalFileSystemFileStorage = 5;
    }
}
