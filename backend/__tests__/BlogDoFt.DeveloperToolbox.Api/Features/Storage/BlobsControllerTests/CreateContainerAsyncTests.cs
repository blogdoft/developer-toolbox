using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.BlobsControllerTests;

public class CreateContainerAsyncTests : BaseBlobsControllerTests
{
    [Fact]
    public async Task Should_CreateContainerAndReturnCreated_When_CalledWithName()
    {
        var container = Fake.Lorem.Word();

        var result = await Sut.CreateContainerAsync(container, CancellationToken.None);

        result.ShouldBeOfType<CreatedResult>();
        await Service.Received(1).CreateContainerAsync(container, Arg.Any<CancellationToken>());
    }
}
