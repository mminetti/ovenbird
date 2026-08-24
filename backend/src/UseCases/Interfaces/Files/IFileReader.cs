namespace UseCases.Interfaces.Files;

public interface IFileReader
{
    Task<IEnumerable<T>> GetRecordsAsync<T>(string fileName, CancellationToken ct);
}
