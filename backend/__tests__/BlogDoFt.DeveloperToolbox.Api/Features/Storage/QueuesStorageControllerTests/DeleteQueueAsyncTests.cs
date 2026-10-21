using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.QueuesStorageControllerTests;

public class DeleteQueueAsyncTests : BaseQueuesStorageControllerTests
{
    [Fact]
    public async Task Should_DeleteQueueAndReturnNoContent_When_CalledWithName()
    {
        var queue = Fake.Lorem.Word();

        var result = await Sut.DeleteQueueAsync(queue, CancellationToken.None);

        result.ShouldBeOfType<NoContentResult>();
        await Service.Received(1).DeleteQueueAsync(queue, Arg.Any<CancellationToken>());
    }
}
