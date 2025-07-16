namespace ConcentrateOn.Core.Data;

public interface IDataSource<T>
{
    Task<List<T>> ReadAsync();
    Task WriteAsync(List<T> content);
}
