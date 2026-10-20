namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

/// <summary>
/// A subscription filter rule and its definition.
/// </summary>
public record RuleInfo(string Name, RuleSettings Properties);
