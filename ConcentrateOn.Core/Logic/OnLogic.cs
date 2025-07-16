using ConcentrateOn.Core.Enums;
using ConcentrateOn.Core.Extensions;
using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Logic;

public class OnLogic(
  IContextual<Subject> subjectContext
, IContextual<Day> daysContext
) : IOnLogically
{
    public Task BeginAsync() => Task.WhenAll(
          subjectContext.InitAsync()
        , daysContext.InitAsync()
        );

    public (bool, Subject?) TryGetSubject(string name)
    {
        var subject = subjectContext.TryFind(name);
        
        return (subject is not null, subject);
    }

    public Guid UpdateExistingSubject(Subject existing, OnRequest request)
    {
        var updated = new Subject(
              existing.Id
            , existing.Name
            , request.Priority ?? existing.Priority
            , request.During   ?? existing.During
            , request.Duration ?? existing.Duration
        );
        subjectContext.Update(updated);

        return updated.Id;
    }

    public Guid CreateNewSubject(OnRequest fromRequest)
    {
        var created = new Subject(
              Guid.NewGuid()
            , fromRequest.Name
            , fromRequest.Priority ?? 1
            , fromRequest.During   ?? During.Afternoon
            , fromRequest.Duration ?? "15m"
            );
        subjectContext.Create(created);

        return created.Id;
    }

    public List<Day> AssociateSubjectToDays(Guid subjectId, List<DayOfWeek> givenDays)
    {
        var updatedDays = new List<Day>();
        foreach (var desired in givenDays)
        {
            var day = daysContext.TryFind(desired.ToString());
            if (day is not null)
            {
                if (!day.SubjectIds.Contains(subjectId))
                {
                    day.SubjectIds.Add(subjectId);
                    daysContext.Update(day);
                }
            }
            else
            {
                day = new Day(
                      Guid.NewGuid()
                    , desired
                    , [subjectId]
                    );
                daysContext.Create(day);
            }
            
            updatedDays.Add(day);
        }

        return updatedDays;
    }

    public List<Day> UnassociateUnwantedDays(Guid subjectId, List<Day> updatedDays)
    {
        var complimentDays = daysContext.All()
            .Subtract(updatedDays)
            .Where(d => d.SubjectIds.Contains(subjectId))
            .ToList();
        foreach (var unassociated in complimentDays)
        {
            unassociated.SubjectIds.Remove(subjectId);
            daysContext.Update(unassociated);
        }

        return complimentDays;
    }

    public Task EndAsync() => Task.WhenAll(
          subjectContext.SaveAsync()
        , daysContext.SaveAsync()
        );
}
