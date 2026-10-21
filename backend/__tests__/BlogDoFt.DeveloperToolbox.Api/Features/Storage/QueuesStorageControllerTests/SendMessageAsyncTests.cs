using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.QueuesStorageControllerTests;

public class SendMessageAsyncTests : BaseQueuesStorageControllerTests
{
    [Fact]
    public async Task Should_SendTextAndReturnCreated_When_RequestIsValid()
    {
        var queue = Fake.Lorem.Word();
        var request = new SendQueueMessageRequest(Fake.Lorem.Sentence());

        var result = await Sut.SendMessageAsync(queue, request, CancellationToken.None);

        result.ShouldBeOfType<CreatedResult>();
        await Service.Received(1).SendMessageAsync(queue, request.Text, Arg.Any<CancellationToken>());
    }
}
