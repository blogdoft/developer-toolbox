using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.BlobsControllerTests;

public class GetContainersAsyncTests : BaseBlobsControllerTests
{
    [Fact]
    public async Task Should_ReturnContainers_When_ContainersExist()
    {
        IReadOnlyList<BlobContainerSummary> containers = [new(Fake.Lorem.Word(), null, null)];
        Service.GetContainersAsync(Arg.Any<CancellationToken>()).Returns(containers);

        var result = await Sut.GetContainersAsync(CancellationToken.None);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(containers);
    }
}
