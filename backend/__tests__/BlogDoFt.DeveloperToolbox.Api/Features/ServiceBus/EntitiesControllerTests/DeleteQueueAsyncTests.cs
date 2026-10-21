using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class DeleteQueueAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNoContent_When_DeleteQueueSucceeds()
    {
        var queueName = Fake.Lorem.Word();

        var result = await Sut.DeleteQueueAsync(queueName, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<NoContentResult>();
        await Namespace.Received(1).DeleteQueueAsync(queueName, Arg.Any<CancellationToken>());
    }
}
