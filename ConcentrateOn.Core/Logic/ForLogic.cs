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

  public List<DayOfWeek> ParsePossibleDays(string requested)
  {
    if (Enum.TryParse(requested, out DayOfWeek desiredDay))
      return [desiredDay];

    List<DayOfWeek> desiredDays = requested switch
    { "today"     => [DateTime.Now.DayOfWeek]
    , "yesterday" => [DateTime.Now.AddDays(-1).DayOfWeek]
    , "tomorrow"  => [DateTime.Now.AddDays(+1).DayOfWeek]
    , "week"      => [ DayOfWeek.Sunday
                     , DayOfWeek.Monday
                     , DayOfWeek.Tuesday
                     , DayOfWeek.Wednesday
                     , DayOfWeek.Thursday
                     , DayOfWeek.Friday
                     , DayOfWeek.Saturday
                     ]
    , _           => throw new ArgumentOutOfRangeException(nameof(requested), requested, 
      "Given value for 'day' not valid. Must be one of a day of the week, or one of 'today', 'tomorrow', 'yesterday'.")
    };

    return desiredDays;
  }

  public Day? GetDayByName(DayOfWeek? possibleDay) =>
    dayContext.TryFind(possibleDay?.ToString() ?? string.Empty);

  static string FillSpace(int count) =>
    count > 0
      ? Enumerable.Range(0, count - 1)
        .Select(_ => ".")
        .Aggregate(string.Empty, (acc, str) => acc + str)
      : string.Empty;

  static StringBuilder AppendSubjectLine(Subject toPrint, int longestName, StringBuilder builder)
  {
    builder.Append($"\t{toPrint.Priority}. ");
    builder.Append($"{toPrint.Name} ");
    if (toPrint.Name.Length != longestName)
      builder.Append(FillSpace(longestName - toPrint.Name.Length) + ' ');

    builder.Append($"- {toPrint.During}, {toPrint.Duration}");
    builder.AppendLine();

    return builder;
  }

  public string ComposeDaySubjectsString(Day? desiredDay)
  {
    var subjectsBuilder = new StringBuilder();
    subjectsBuilder.AppendLine(OUTPUT_SEPERATOR);
    if (desiredDay is not null
    && desiredDay.SubjectIds.Count > 0)
    {
      subjectsBuilder.Append($"Desired Subjects for {desiredDay.Name}");
      subjectsBuilder.AppendLine(
        desiredDay.Name == DateTime.Now.DayOfWeek
          ? "    <<== TODAY"
          : string.Empty
          );

      var subjects = desiredDay.SubjectIds
        .Select(subjectContext.TryFind)
        .Where(s => s is not null)
        .OfType<Subject>()
        .OrderBy(s => s.Priority)
        .ToList();
      var longestName = subjects
        .MaxBy(s => s.Name.Length)!
        .Name.Length;
      subjectsBuilder = subjects
        .Aggregate(subjectsBuilder, (current, subject) =>
          AppendSubjectLine(subject, longestName, current));
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
