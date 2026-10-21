using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.BlobsControllerTests;

public class DeleteBlobAsyncTests : BaseBlobsControllerTests
{
    [Fact]
    public async Task Should_DeleteBlobAndReturnNoContent_When_CalledWithName()
    {
        var (container, name) = (Fake.Lorem.Word(), Fake.System.FileName());

        var result = await Sut.DeleteBlobAsync(container, name, CancellationToken.None);

        result.ShouldBeOfType<NoContentResult>();
        await Service.Received(1).DeleteBlobAsync(container, name, Arg.Any<CancellationToken>());
    }
}
