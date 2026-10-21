using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.Iso8601DurationAttributeTests;

public class ParseTests
{
    [Theory]
    [InlineData("PT1H", 60)]
    [InlineData("P14D", 20160)]
    [InlineData("P1DT2H30M", 1590)]
    public void Should_ReturnTimeSpan_When_ValueIsIso8601Duration(string value, int totalMinutes)
    {
        var result = Iso8601DurationAttribute.Parse(value);

        result.ShouldBe(TimeSpan.FromMinutes(totalMinutes));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_ReturnNull_When_ValueIsNullOrBlank(string? value)
    {
        var result = Iso8601DurationAttribute.Parse(value);

        result.ShouldBeNull();
    }

    [Fact]
    public void Should_ThrowFormatException_When_ValueIsNotIso8601Duration()
    {
        var action = () => { Iso8601DurationAttribute.Parse("one hour"); };

        action.ShouldThrow<FormatException>();
    }
}
