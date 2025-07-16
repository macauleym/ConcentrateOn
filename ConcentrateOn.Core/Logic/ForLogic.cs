using System.Text;
using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Models;

namespace ConcentrateOn.Core.Logic;

public class ForLogic(
  IContextual<Subject> subjectContext
, IContextual<Day> dayContext
) : IForLogically 
{
  const string OUTPUT_SEPERATOR = "*****************************";

  public Task BeginAsync() => Task.WhenAll(
      subjectContext.InitAsync()
    , dayContext.InitAsync()
    );

  public DayOfWeek? ParsePossibleDayOfWeek(string requested)
  {
    if (!Enum.TryParse(requested, out DayOfWeek desiredDay))
    {
      desiredDay = requested switch
      { "today"     => DateTime.Now.DayOfWeek
      , "yesterday" => DateTime.Now.AddDays(-1).DayOfWeek
      , "tomorrow"  => DateTime.Now.AddDays(+1).DayOfWeek
      , _           => throw new ArgumentOutOfRangeException(nameof(requested), requested, "Given value for 'day' not valid. Must be one of a day of the week, or one of 'today', 'tomorrow', 'yesterday'.")
      };
    }

    return desiredDay;
  }

  public Day? GetDayByName(DayOfWeek? possibleDay) =>
    dayContext.TryFind(possibleDay?.ToString() ?? string.Empty);
  
  public string ComposeDaySubjectsString(Day? desiredDay)
  {
    var subjectsBuilder = new StringBuilder();
    subjectsBuilder.AppendLine(OUTPUT_SEPERATOR);
    if (desiredDay is not null
    && desiredDay.SubjectIds.Count > 0)
    {
      subjectsBuilder.AppendLine($"Desired Subjects for {desiredDay.Name}");
      for (var i = 0; i < desiredDay.SubjectIds.Count; i++)
      {
        var subject = subjectContext.TryFind(desiredDay.SubjectIds[i]);
        if (subject is not null)
          subjectsBuilder.AppendLine($"\t{i + 1}. {subject.Name} - {subject.During}, {subject.Duration}");
      }
    }
    else 
      subjectsBuilder.AppendLine("<< No subjects for this day >>");

    subjectsBuilder.AppendLine(OUTPUT_SEPERATOR);

    return subjectsBuilder.ToString();
  }

  public Task EndAsync() => Task.WhenAll(
      subjectContext.SaveAsync()
    , dayContext.SaveAsync()
    );
}
