namespace Core.Common;

public static partial class Constants
{
    public static class ConnectorImplementations
    {
        public const string FluentFtpService = "FluentFtpService";
        public const string SshNetSftpService = "SshNetSftpService";
        public const string LocalFileSystemFtpService = "LocalFileSystemFtpService";
        public const string AzureBlobFileStorage = "AzureBlobFileStorage";
        public const string LocalFileSystemFileStorage = "LocalFileSystemFileStorage";
    }
}
