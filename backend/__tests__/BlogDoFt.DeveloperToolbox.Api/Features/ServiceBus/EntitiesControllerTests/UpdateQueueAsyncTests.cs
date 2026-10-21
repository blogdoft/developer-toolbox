using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class UpdateQueueAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNoContent_When_UpdateQueueSucceeds()
    {
        var queueName = Fake.Lorem.Word();
        var settings = new QueueSettings();

        var result = await Sut.UpdateQueueAsync(queueName, settings, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<NoContentResult>();
        await Namespace.Received(1).UpdateQueueAsync(queueName, settings, Arg.Any<CancellationToken>());
    }
}
