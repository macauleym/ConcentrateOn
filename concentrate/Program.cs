using concentrate.Commands;
using ConcentrateOn.Core.Data;

var subjectsContext = new SubjectContext(new FileSource("subjects.json"));
var daysContext     = new DaysContext(new FileSource("days.json"));

await ConcentrateCommand.Create(subjectsContext, daysContext)
    .Parse(args)
    .InvokeAsync();
    