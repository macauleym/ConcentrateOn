namespace ConcentrateOn.Core.Models;

public record Day(
  Guid Id  
, DayOfWeek Name
, List<Guid> SubjectIds
);
