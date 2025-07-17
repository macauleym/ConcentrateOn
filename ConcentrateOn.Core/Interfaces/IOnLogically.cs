using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Interfaces;

public interface IOnLogically : ITransactable
{
    bool TryGetSubject(string name, out Subject subject);
    Guid UpdateExistingSubject(Subject existing, OnRequest request);
    Guid CreateNewSubject(OnRequest fromRequest);
    List<DayOfWeek> ValidateAssociations(string? requestedDays, bool? isForget);
    List<Day> AssociateSubjectToDays(Guid subjectId, List<DayOfWeek> givenDays);
    List<Day> UnassociateUnwantedDays(Guid subjectId, List<Day> complimentDays);
}
