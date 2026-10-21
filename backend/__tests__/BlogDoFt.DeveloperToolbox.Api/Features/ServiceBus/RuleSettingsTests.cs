using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using System.ComponentModel.DataAnnotations;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus;

public class RuleSettingsTests
{
    [Theory]
    [InlineData("True")]
    [InlineData("false")]
    public void Should_BeValid_When_FilterTypeNeedsNoExtraData(string filterType)
    {
        var errors = Validate(new RuleSettings { FilterType = filterType });

        errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Unknown")]
    public void Should_ReportFilterType_When_FilterTypeIsNotSupported(string? filterType)
    {
        var errors = Validate(new RuleSettings { FilterType = filterType });

        errors.Single().MemberNames.ShouldBe([nameof(RuleSettings.FilterType)]);
    }

    [Fact]
    public void Should_ReportSqlExpression_When_SqlFilterHasNoExpression()
    {
        var errors = Validate(new RuleSettings { FilterType = "sql", SqlExpression = " " });

        errors.Single().MemberNames.ShouldBe([nameof(RuleSettings.SqlExpression)]);
    }

    [Fact]
    public void Should_BeValid_When_SqlFilterHasExpression()
    {
        var errors = Validate(new RuleSettings { FilterType = "Sql", SqlExpression = "1=1" });

        errors.ShouldBeEmpty();
    }

    [Fact]
    public void Should_ReportFilterType_When_CorrelationFilterHasNothingToMatch()
    {
        var errors = Validate(new RuleSettings { FilterType = "Correlation" });

        errors.Single().MemberNames.ShouldBe([nameof(RuleSettings.FilterType)]);
    }

    [Fact]
    public void Should_BeValid_When_CorrelationFilterHasApplicationProperty()
    {
        var settings = new RuleSettings
        {
            FilterType = "Correlation",
            ApplicationProperties = new() { ["k"] = "v" },
        };

        var errors = Validate(settings);

        errors.ShouldBeEmpty();
    }

    [Fact]
    public void Should_HaveCorrelationValues_When_AnyCorrelationPropertyIsSet()
    {
        var settings = new RuleSettings { Subject = "orders" };

        settings.HasCorrelationValues.ShouldBeTrue();
    }

    [Fact]
    public void Should_NotHaveCorrelationValues_When_PropertiesAreBlank()
    {
        var settings = new RuleSettings { Subject = " ", ApplicationProperties = [] };

        settings.HasCorrelationValues.ShouldBeFalse();
    }

    private static List<ValidationResult> Validate(RuleSettings settings)
        => settings.Validate(new ValidationContext(settings)).ToList();
}
