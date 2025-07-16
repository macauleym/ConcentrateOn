using System.CommandLine;
using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Models;

namespace concentrate.Commands.For;

public class ForCommand(IForLogically logically) {
    public const string Name = "for";

    readonly Argument<string> dayArgument = new("Day");

    ForRequest GetRequestParams(ParseResult parsed)
    {
        var day = parsed.GetRequiredValue(dayArgument);

        return new ForRequest(day);
    }

    async Task ForHandler(ParseResult parsed)
    {
        var request = GetRequestParams(parsed);

        await logically.BeginAsync();
        
        var possibleDay = logically.ParsePossibleDayOfWeek(request.Day);
        if (!possibleDay.HasValue)
        {
            Console.WriteLine($"Can't attempt to get subjects for day that cannot be mapped.\nGiven value {request.Day}.\nExpected a valid day of the week, or one of 'today', 'yesterday', or 'tomorrow'.");
            return;
        }

        var desiredDay = logically.GetDayByName(possibleDay);
        Console.WriteLine(
             logically.ComposeDaySubjectsString(desiredDay));

        await logically.EndAsync();
    }

    public Command Create()
    {
        var @for = new Command(Name);
        @for.Arguments.Add(dayArgument);
        @for.SetAction(ForHandler);

        return @for;
    }
}
