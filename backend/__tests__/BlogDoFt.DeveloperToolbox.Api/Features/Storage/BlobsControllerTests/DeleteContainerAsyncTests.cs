using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.BlobsControllerTests;

public class DeleteContainerAsyncTests : BaseBlobsControllerTests
{
    [Fact]
    public async Task Should_DeleteContainerAndReturnNoContent_When_CalledWithName()
    {
        var container = Fake.Lorem.Word();

        var result = await Sut.DeleteContainerAsync(container, CancellationToken.None);

        result.ShouldBeOfType<NoContentResult>();
        await Service.Received(1).DeleteContainerAsync(container, Arg.Any<CancellationToken>());
    }
}
