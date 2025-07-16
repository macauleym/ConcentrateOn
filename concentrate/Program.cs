using concentrate.Commands;
using ConcentrateOn.Core.Data;
using ConcentrateOn.Core.Models;

var subjectsContext = new SubjectContext(
    new FileSource<Subject>("subjects.json"));
var daysContext     = new DaysContext(
    new FileSource<Day>("days.json"));

await ConcentrateCommand.Create(subjectsContext, daysContext)
    .Parse(args)
    .InvokeAsync();
    