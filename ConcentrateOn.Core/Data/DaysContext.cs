using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Data;

public class DaysContext(IDataSource<Day> daysSource) : IContextual<Day>
{
    List<Day> days = [];
    
    public async Task InitAsync()
    {
        days = await daysSource.ReadAsync();
    }

    public List<Day> All() =>
        days;

    public Day? TryFind(Guid id) =>
        days.Find(d => d.Id == id);

    public Day? TryFind(string name) =>
        days.Find(d => d.Name.ToString() == name);

    public Guid Create(Day toCreate)
    {
        days.Add(toCreate);

        return toCreate.Id;
    }

    public Guid Update(Day toUpdate)
    {
        var updateIndex   = days.FindIndex(d => d.Id == toUpdate.Id);
        days[updateIndex] = toUpdate;

        return toUpdate.Id;
    }

    public void Remove(Guid id)
    {
        var targetIndex = days.FindIndex(d => d.Id == id);
        if (targetIndex >= 0)
            days.RemoveAt(targetIndex);
    }

    public Task SaveAsync() =>
        daysSource.WriteAsync(days);
}
