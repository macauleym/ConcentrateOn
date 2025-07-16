using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Interfaces;

public interface IOnLogically : ITransactable
{
    (bool, Subject?) TryGetSubject(string name);
    Guid UpdateExistingSubject(Subject existing, OnRequest request);
    Guid CreateNewSubject(OnRequest fromRequest);
    List<Day> AssociateSubjectToDays(Guid subjectId, List<DayOfWeek> givenDays);
    List<Day> UnassociateUnwantedDays(Guid subjectId, List<Day> complimentDays);
}
