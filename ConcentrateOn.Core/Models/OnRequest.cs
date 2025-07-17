using ConcentrateOn.Core.Enums;

namespace ConcentrateOn.Core.Models;

public record OnRequest(
  string Name
, int? Priority
, During? During
, string? Duration
, string? Days
, bool? IsForget 
);
