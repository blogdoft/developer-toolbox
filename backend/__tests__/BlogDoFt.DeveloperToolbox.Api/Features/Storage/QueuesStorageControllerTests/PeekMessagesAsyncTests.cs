using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.QueuesStorageControllerTests;

public class PeekMessagesAsyncTests : BaseQueuesStorageControllerTests
{
    [Fact]
    public async Task Should_ReturnPeekedMessages_When_QueueHasMessages()
    {
        var queue = Fake.Lorem.Word();
        IReadOnlyList<StorageQueueMessage> messages = [new("id", "body", null, null, 0)];
        Service.PeekMessagesAsync(queue, 7, Arg.Any<CancellationToken>()).Returns(messages);

        var result = await Sut.PeekMessagesAsync(queue, 7, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(messages);
    }
}
