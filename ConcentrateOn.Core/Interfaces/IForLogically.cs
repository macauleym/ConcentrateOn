using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Interfaces;

public interface IForLogically
{
    DayOfWeek? ParsePossibleDayOfWeek(string requested);
    Task<Day?> GetDayByNameAsync(DayOfWeek? possibleDay);
    Task<string> ComposeDaySubjectsStringAsync(Day? desiredDay);
}
