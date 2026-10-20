using System.ComponentModel.DataAnnotations;

namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

/// <summary>
/// A subscription rule: a filter (<c>Sql</c>, <c>Correlation</c>, <c>True</c> or <c>False</c>)
/// and an optional SQL action.
/// </summary>
public class RuleSettings : IValidatableObject
{
    public static readonly string[] FilterTypes = ["Sql", "Correlation", "True", "False"];

    public string? FilterType { get; init; }

    public string? SqlExpression { get; init; }

    public string? ActionExpression { get; init; }

    public string? CorrelationId { get; init; }

    public string? MessageId { get; init; }

    public string? To { get; init; }

    public string? ReplyTo { get; init; }

    public string? Subject { get; init; }

    public string? SessionId { get; init; }

    public string? ReplyToSessionId { get; init; }

    public string? ContentType { get; init; }

    public Dictionary<string, string>? ApplicationProperties { get; init; }

    public bool HasCorrelationValues => ApplicationProperties is { Count: > 0 }
        || new[] { CorrelationId, MessageId, To, ReplyTo, Subject, SessionId, ReplyToSessionId, ContentType }
            .Any(v => !string.IsNullOrWhiteSpace(v));

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!FilterTypes.Contains(FilterType, StringComparer.OrdinalIgnoreCase))
        {
            yield return new ValidationResult(
                $"{nameof(FilterType)} must be one of: {string.Join(", ", FilterTypes)}.",
                [nameof(FilterType)]);
            yield break;
        }

        if (FilterType!.Equals("Sql", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(SqlExpression))
        {
            yield return new ValidationResult(
                $"{nameof(SqlExpression)} is required for a Sql filter.",
                [nameof(SqlExpression)]);
        }

        if (FilterType.Equals("Correlation", StringComparison.OrdinalIgnoreCase) && !HasCorrelationValues)
        {
            yield return new ValidationResult(
                "A Correlation filter needs at least one property to match.",
                [nameof(FilterType)]);
        }
    }
}
