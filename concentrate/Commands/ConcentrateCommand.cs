using System.CommandLine;
using concentrate.Commands.For;
using concentrate.Commands.On;
using ConcentrateOn.Core.Interfaces;
using ConcentrateOn.Core.Logic;
using ConcentrateOn.Core.Models;

namespace concentrate.Commands;

public static class ConcentrateCommand
{
    public static Command Create(IContextual<Subject> subjectContext, IContextual<Day> dayContext) =>
        new CommandBuilder(new RootCommand(
            description: "Adds topics/subjects to concentrate on, for the desired day(s)."))
        .WithSubCommand(new OnCommand(new OnLogic(
              subjectContext
            , dayContext))
            .Create())
        .WithSubCommand(new ForCommand(new ForLogic(
              subjectContext
            , dayContext))
            .Create())
        .Build();
}
