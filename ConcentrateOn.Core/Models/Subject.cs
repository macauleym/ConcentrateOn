using ConcentrateOn.Core.Enums;

namespace ConcentrateOn.Core.Models;

public record Subject(
  Guid Id
, string Name
, int Priority
, During During
, string Duration
);
