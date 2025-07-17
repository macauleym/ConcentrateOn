using System.CommandLine;
using ConcentrateOn.Core.Enums;
using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Models;

namespace concentrate.Commands.On;

public class OnCommand(IOnLogically logically) {
    public const string Name = "on";

    readonly Argument<string> subjectArgument = new("Subject");
    readonly Option<int> priorityOption       = new("Priority", "--priority", "-p");
    readonly Option<string> daysOption        = new("Days", "--days");
    readonly Option<During> duringOption      = new("During", "--during", "-d");
    readonly Option<string> durationOption    = new("Duration", "--duration", "-t");
    readonly Option<bool> forgetOption        = new("Forget", "--forget", "-r");

    OnRequest GetRequestParams(ParseResult parsed) =>
        new ( parsed.GetRequiredValue(subjectArgument)
            , parsed.GetValue(priorityOption)
            , parsed.GetValue(duringOption)
            , parsed.GetValue(durationOption)
            , parsed.GetValue(daysOption)
            , parsed.GetValue(forgetOption)
            );
    
    async Task OnHandler(ParseResult parsed)
    {
        var request = GetRequestParams(parsed);
        
        await logically.BeginAsync();
        
        var subjectId = logically.TryGetSubject(request.Name, out var existing)
            ? logically.UpdateExistingSubject(existing, request) 
            : logically.CreateNewSubject(request);

        var associatedDays = new List<DayOfWeek>();
        try
        {
            associatedDays = logically.ValidateAssociations(request.Days, request.IsForget);
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(e.Message);
            return;
        }
        finally
        {
            await logically.EndAsync();
        }

        var updatedDays      = logically.AssociateSubjectToDays(subjectId, associatedDays);
        var unassociatedDays = logically.UnassociateUnwantedDays(subjectId, updatedDays);
        
        await logically.EndAsync();
    }

    public Command Create()
    {
        var on = new Command(Name);
        on.Arguments.Add(subjectArgument);
        on.Options.Add(daysOption);
        on.Options.Add(duringOption);
        on.Options.Add(durationOption);
        on.Options.Add(priorityOption);
        on.Options.Add(forgetOption);
        on.SetAction(OnHandler);

        return on;
    }
}
