namespace UseCases.Interfaces.Files;

public class FileOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string RootDirectory { get; set; } = string.Empty;
    public string Implementation { get; set; } = string.Empty;
}
