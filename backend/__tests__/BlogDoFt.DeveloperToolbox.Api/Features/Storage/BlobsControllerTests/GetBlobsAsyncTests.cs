using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.BlobsControllerTests;

public class GetBlobsAsyncTests : BaseBlobsControllerTests
{
    [Fact]
    public async Task Should_ReturnBlobsFilteredByPrefix_When_PrefixIsGiven()
    {
        var (container, prefix) = (Fake.Lorem.Word(), Fake.Lorem.Word());
        IReadOnlyList<BlobSummary> blobs = [new($"{prefix}/a.txt", 10, "text/plain", null)];
        Service.GetBlobsAsync(container, prefix, Arg.Any<CancellationToken>()).Returns(blobs);

        var result = await Sut.GetBlobsAsync(container, prefix, CancellationToken.None);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(blobs);
    }
}
