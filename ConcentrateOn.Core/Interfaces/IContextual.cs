namespace ConcentrateOn.Core.Interfaces;

public interface IContextual<T>
{
    Task<T?> GetByAsync(Guid id);
    Task<T?> GetByAsync(string name);
    Task<List<T>> GetAllAsync();
    Task<Guid> ResolveAsync(T target);
    Task DeleteAsync(Guid id);
}
