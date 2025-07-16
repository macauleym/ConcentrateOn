using System.Text.Json;
using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Data;

public class DaysContext(IDataSource source) : IContextual<Day>
{
    public async Task<Day?> GetByAsync(Guid id)
    {
        var text = await source.ReadAsync();
        if (string.IsNullOrWhiteSpace(text))
            return null;

        return JsonSerializer.Deserialize<List<Day>>(text)
            !.FirstOrDefault(s => s.Id == id);
    }

    public async Task<Day?> GetByAsync(string name)
    {
        var text = await source.ReadAsync();
        if (string.IsNullOrWhiteSpace(text))
            return null;

        return JsonSerializer.Deserialize<List<Day>>(text)
            !.FirstOrDefault(d => d.Name == Enum.Parse<DayOfWeek>(name));
    }

    public async Task<List<Day>> GetAllAsync()
    {
        var text = await source.ReadAsync();
        if (string.IsNullOrWhiteSpace(text))
            return [];

        return JsonSerializer.Deserialize<List<Day>>(text)!;
    }

    public async Task<Guid> ResolveAsync(Day target)
    {
        var allSubjects = await GetAllAsync();
        var existing    = allSubjects.Find(s => s.Id == target.Id);
        if (existing is null)
            allSubjects.Add(target);
        else
        {
            var updated = new Day(
                  existing.Id
                , existing.Name
                , target.SubjectIds
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
