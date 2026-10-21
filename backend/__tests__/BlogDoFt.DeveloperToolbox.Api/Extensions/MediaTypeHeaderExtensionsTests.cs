using BlogDoFt.DeveloperToolbox.Api.Extensions;
using Microsoft.Net.Http.Headers;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Extensions;

public class MediaTypeHeaderExtensionsTests
{
    [Fact]
    public void Should_ReturnTrue_When_MediaTypeIsPresent()
    {
        IList<MediaTypeHeaderValue> headers = [new("application/json"), new("text/plain")];

        var result = headers.Includes("text/plain");

        result.ShouldBeTrue();
    }

    [Fact]
    public void Should_ReturnTrue_When_MediaTypeDiffersOnlyByCase()
    {
        IList<MediaTypeHeaderValue> headers = [new("Text/Plain")];

        var result = headers.Includes("text/plain");

        result.ShouldBeTrue();
    }

    [Fact]
    public void Should_ReturnFalse_When_MediaTypeIsAbsent()
    {
        IList<MediaTypeHeaderValue> headers = [new("application/json")];

        var result = headers.Includes("text/plain");

        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_ReturnFalse_When_MediaTypeIsNullOrEmpty(string? mediaType)
    {
        IList<MediaTypeHeaderValue> headers = [new("text/plain")];

        var result = headers.Includes(mediaType!);

        result.ShouldBeFalse();
    }
}
