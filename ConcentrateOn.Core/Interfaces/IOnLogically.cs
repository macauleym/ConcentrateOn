using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Interfaces;

public interface IOnLogically
{
    Task<(bool, Subject?)> TryGetExistingAsync(string name);

    Task<Guid> UpdateExistingAsync(Subject existing, OnRequest request);

    Task<Guid> CreateNewAsync(OnRequest request);

    Task<List<Day>> AssociateDaysAsync(string days, Guid subjectId);

    Task RemoveUnwantedDaysAsync(List<Day> complimentDays, Guid subjectId);
}