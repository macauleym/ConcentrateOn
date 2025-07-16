using ConcentrateOn.Core.Enums;
using ConcentrateOn.Core.Extensions;
using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Logic;

public class OnLogic(
  IContextual<Subject> subjectContext
, IContextual<Day> dayContext
) : IOnLogically
{
    public async Task<(bool, Subject?)> TryGetExistingAsync(string name)
    {
        var subject = await subjectContext.GetByAsync(name);
        
        return (subject is not null, subject);
    }
    
    public async Task<Guid> UpdateExistingAsync(Subject existing, OnRequest request)
    {
        var updated = new Subject(
              existing.Id
            , existing.Name
            , request.Priority ?? existing.Priority
            , request.During   ?? existing.During
            , request.Duration ?? existing.Duration
            );
        await subjectContext.ResolveAsync(updated);
        
        return updated.Id;
    }

    public async Task<Guid> CreateNewAsync(OnRequest request) 
    {
        var created = new Subject(
              Guid.NewGuid()
            , request.Name
            , request.Priority ?? 1
            , request.During   ?? During.Afternoon
            , request.Duration ?? "15m"
        );
        await subjectContext.ResolveAsync(created);
        
        return created.Id;
    }

    public async Task<List<Day>> AssociateDaysAsync(string days, Guid subjectId)
    {
        var desiredDays = days.Split(',');
        var updatedDays = new List<Day>();
        var allDays     = await dayContext.GetAllAsync();
        foreach (var d in desiredDays)
        {
            var day = await dayContext.GetByAsync(d);
            if (day is not null)
            {
                // Next we need to check if the given day(s) have this subject
                // associated.
                // If they do, no action needed.
                // If not, we need to at the target's Id
                // the the list of subject ids.
                if (!day.SubjectIds.Contains(subjectId))
                    day.SubjectIds.Add(subjectId);
            }
            else
            {
                day = new Day(
                      Guid.NewGuid()
                    , Enum.Parse<DayOfWeek>(d)
                    , [subjectId]
                    );
            }
            
            await dayContext.ResolveAsync(day);
            updatedDays.Add(day);
        }

        var complimentDays = allDays.Subtract(updatedDays);
        
        return complimentDays;
    }

    public async Task RemoveUnwantedDaysAsync(List<Day> complimentDays, Guid subjectId)
    {
        foreach (var c in complimentDays)
        {
            c.SubjectIds.Remove(subjectId);
            await dayContext.ResolveAsync(c);
        }
    }
}
