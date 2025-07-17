using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Interfaces;

public interface IForLogically : ITransactable
{
    List<DayOfWeek> ParsePossibleDays(string requested);
    Day? GetDayByName(DayOfWeek? possibleDay);
    string ComposeDaySubjectsString(Day? desiredDay);
}
