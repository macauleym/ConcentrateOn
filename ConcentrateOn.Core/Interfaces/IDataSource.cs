namespace ConcentrateOn.Core.Data;

public interface IDataSource
{
    Task<string> ReadAsync();
    Task WriteAsync(string content);
}
