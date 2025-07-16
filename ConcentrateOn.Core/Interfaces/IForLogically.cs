using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Interfaces;

public interface IForLogically : ITransactable
{
    DayOfWeek? ParsePossibleDayOfWeek(string requested);
    Day? GetDayByName(DayOfWeek? possibleDay);
    string ComposeDaySubjectsString(Day? desiredDay);
}
