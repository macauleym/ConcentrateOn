using System.CommandLine;
using System.Linq;
using ConcentrateOn.Core.Enums;
using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Models;

namespace concentrate.Commands.On;

public class OnCommand(IOnLogically logically) {
    public const string Name = "on";

    readonly Argument<string> subjectArgument = new("Subject");
    readonly Option<string> daysOption        = new("Days", "--days");
    readonly Option<During> duringOption      = new("During", "--during");
    readonly Option<string> durationOption    = new("Duration", "--duration");
    readonly Option<int> priorityOption       = new("Priority", "--priority");

    OnRequest GetRequestParams(ParseResult parsed) =>
        new ( parsed.GetRequiredValue(subjectArgument)
            , parsed.GetValue(priorityOption)
            , parsed.GetValue(duringOption)
            , parsed.GetValue(durationOption)
            , parsed.GetValue(daysOption)
            );

    async Task OnHandler(ParseResult parsed)
    {
        var request            = GetRequestParams(parsed);
        var (exists, existing) = await logically.TryGetExistingAsync(request.Name);
        var subjectId          = exists? 
            await logically.UpdateExistingAsync(existing!, request) 
            : await logically.CreateNewAsync(request);
        
        var complimentDays = await logically.AssociateDaysAsync(request.Days, subjectId);
        await logically.RemoveUnwantedDaysAsync(complimentDays, subjectId);
    }

    public Command Create()
    {
        var on = new Command(Name);
        on.Arguments.Add(subjectArgument);
        on.Options.Add(daysOption);
        on.Options.Add(duringOption);
        on.Options.Add(durationOption);
        on.Options.Add(priorityOption);
        on.SetAction(OnHandler);

        return on;
    }
}
