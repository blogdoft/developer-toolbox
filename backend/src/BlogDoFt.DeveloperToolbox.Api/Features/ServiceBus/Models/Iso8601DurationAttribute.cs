using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

/// <summary>
/// Validates that a string is an ISO 8601 duration (for example <c>PT1H</c> or <c>P14D</c>).
/// Null or empty values are valid.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class Iso8601DurationAttribute : ValidationAttribute
{
    public Iso8601DurationAttribute()
        : base("The field {0} must be an ISO 8601 duration, such as PT1H or P14D.")
    {
    }

    /// <summary>
    /// Converts an ISO 8601 duration to a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="value">The ISO 8601 duration; null or empty yields null.</param>
    public static TimeSpan? Parse(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : XmlConvert.ToTimeSpan(value);

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to an ISO 8601 duration.
    /// </summary>
    /// <param name="value">The time span; null yields null.</param>
    public static string? Format(TimeSpan? value)
        => value is null ? null : XmlConvert.ToString(value.Value);

    public override bool IsValid(object? value)
    {
        if (value is not string text || string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        try
        {
            XmlConvert.ToTimeSpan(text);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
