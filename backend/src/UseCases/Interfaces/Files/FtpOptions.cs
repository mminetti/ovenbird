namespace UseCases.Interfaces.Files;

public class FtpOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 22;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RemoteDirectory {  get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Implementation { get; set; } = string.Empty;
}
