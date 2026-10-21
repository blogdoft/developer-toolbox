using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.QueuesStorageControllerTests;

public class ClearMessagesAsyncTests : BaseQueuesStorageControllerTests
{
    [Fact]
    public async Task Should_ClearMessagesAndReturnNoContent_When_CalledWithQueue()
    {
        var queue = Fake.Lorem.Word();

        var result = await Sut.ClearMessagesAsync(queue, CancellationToken.None);

        result.ShouldBeOfType<NoContentResult>();
        await Service.Received(1).ClearMessagesAsync(queue, Arg.Any<CancellationToken>());
    }
}
