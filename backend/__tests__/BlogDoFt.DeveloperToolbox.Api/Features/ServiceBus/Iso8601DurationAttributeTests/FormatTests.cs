using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.Iso8601DurationAttributeTests;

public class FormatTests
{
    [Fact]
    public void Should_ReturnIso8601Duration_When_ValueIsTimeSpan()
    {
        var result = Iso8601DurationAttribute.Format(TimeSpan.FromDays(14));

        result.ShouldBe("P14D");
    }

    [Fact]
    public void Should_ReturnNull_When_ValueIsNull()
    {
        var result = Iso8601DurationAttribute.Format(null);

        result.ShouldBeNull();
    }

    [Fact]
    public void Should_RoundTrip_When_FormattedValueIsParsed()
    {
        var original = TimeSpan.FromMinutes(90);

        var result = Iso8601DurationAttribute.Parse(Iso8601DurationAttribute.Format(original));

        result.ShouldBe(original);
    }
}
