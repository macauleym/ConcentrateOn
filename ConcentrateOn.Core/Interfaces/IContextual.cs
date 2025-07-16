namespace ConcentrateOn.Core.Interfaces;

public interface IContextual<T>
{
    Task InitAsync();
    List<T> All();
    T? TryFind(Guid id);
    T? TryFind(string name);
    Guid Create(T toCreate);
    Guid Update(T toUpdate);
    void Remove(Guid id);
    Task SaveAsync();
}
