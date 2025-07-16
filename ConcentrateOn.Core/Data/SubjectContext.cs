using System.Text.Json;
using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Data;

public class SubjectContext(IDataSource source) : IContextual<Subject>
{
    /// <summary>
    /// Returns a Subject by the given Id, or `null` if there isn't one.
    /// </summary>
    /// <param name="id">The Id to query by.</param>
    /// <returns>The matching Subject, or `null` if none exists.</returns>
    public async Task<Subject?> GetByAsync(Guid id)
    {
        var text = await source.ReadAsync();
        if (string.IsNullOrWhiteSpace(text))
            return null;
        
        return JsonSerializer.Deserialize<List<Subject>>(text)
            !.FirstOrDefault(s => s.Id == id);
    }

    public async Task<Subject?> GetByAsync(string name)
    {
        var text = await source.ReadAsync();
        if (string.IsNullOrWhiteSpace(text))
            return null;

        return JsonSerializer.Deserialize<List<Subject>>(text)
            !.FirstOrDefault(s => s.Name == name);
    }

    public async Task<List<Subject>> GetAllAsync()
    {
        var text = await source.ReadAsync();
        if (string.IsNullOrWhiteSpace(text))
            return [];

        return JsonSerializer.Deserialize<List<Subject>>(text)!;
    }

    public async Task<Guid> ResolveAsync(Subject target)
    {
        var allSubjects = await GetAllAsync();
        var existing    = allSubjects.Find(s => s.Id == target.Id);
        if (existing is null)
            allSubjects.Add(target);
        else
        {
            var updated = new Subject(
                  existing.Id
                , existing.Name
                , target.Priority
                , target.During
                , target.Duration
                );
            var updateIndex          = allSubjects.FindIndex(s => s.Id == updated.Id);
            allSubjects[updateIndex] = updated;
        }
        
        await source.WriteAsync(JsonSerializer.Serialize(allSubjects));

        return target.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        var allItems = await GetAllAsync();
        var target   = allItems.Find(s => s.Id == id);
        if (target is not null)
            allItems.Remove(target);
    }
}
