using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.Iso8601DurationAttributeTests;

public class IsValidTests
{
    private readonly Iso8601DurationAttribute _sut = new();

    [Theory]
    [InlineData("PT1H")]
    [InlineData("P14D")]
    [InlineData("P10675199DT2H48M5.4775807S")]
    public void Should_ReturnTrue_When_ValueIsIso8601Duration(string value)
    {
        var result = _sut.IsValid(value);

        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Should_ReturnTrue_When_ValueIsNullOrBlank(string? value)
    {
        var result = _sut.IsValid(value);

        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData("one hour")]
    [InlineData("1h")]
    [InlineData("PTxyz")]
    public void Should_ReturnFalse_When_ValueIsNotIso8601Duration(string value)
    {
        var result = _sut.IsValid(value);

        result.ShouldBeFalse();
    }
}
