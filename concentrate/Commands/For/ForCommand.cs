using System.CommandLine;
using ConcentrateOn.Core.Extensions;
using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Models;

namespace concentrate.Commands.For;

public class ForCommand(IForLogically logically) {
    public const string Name = "for";

    readonly Argument<string> dayArgument = new("Day");

    ForRequest GetRequestParams(ParseResult parsed) =>
        parsed.GetRequiredValue(dayArgument)
        .PipeTo(d => new ForRequest(d));

    async Task ForHandler(ParseResult parsed)
    {
        var request = GetRequestParams(parsed);

        await logically.BeginAsync();
        
        var possibleDays = logically.ParsePossibleDays(request.Day);
        if (possibleDays.Count <= 0)
        {
            Console.WriteLine($"Can't attempt to get subjects for day that cannot be mapped.\nGiven value {request.Day}.\nExpected a valid day of the week, or one of 'today', 'yesterday', 'tomorrow', or 'week'.");
            return;
        }

        foreach (var day in possibleDays)
            logically.GetDayByName(day)
            .PipeTo(logically.ComposeDaySubjectsString)
            .PipeTo(Console.WriteLine);

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
