using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Data;

public class SubjectContext(IDataSource<Subject> subjectData) : IContextual<Subject>
{
    List<Subject> subjects = [];

    public async Task InitAsync()
    {
        subjects = await subjectData.ReadAsync();
    }

    public List<Subject> All() =>
        subjects;

    public Subject? TryFind(Guid id) =>
        subjects.Find(s => s.Id == id);

    public Subject? TryFind(string name) =>
        subjects.Find(s => s.Name == name);

    public Guid Create(Subject toCreate)
    {
        subjects.Add(toCreate);

        return toCreate.Id;
    }

    public Guid Update(Subject toUpdate)
    {
        var updateIndex       = subjects.FindIndex(s => s.Id == toUpdate.Id);
        subjects[updateIndex] = toUpdate;

        return toUpdate.Id;
    }

    public void Remove(Guid id)
    {
        var targetIndex = subjects.FindIndex(s => s.Id == id);
        if (targetIndex >= 0) 
            subjects.RemoveAt(targetIndex);
    }

    public async Task SaveAsync()
    {
        await subjectData.WriteAsync(subjects);
    }
}
