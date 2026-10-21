using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class DeleteTopicAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNoContent_When_DeleteTopicSucceeds()
    {
        var topicName = Fake.Lorem.Word();

        var result = await Sut.DeleteTopicAsync(topicName, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<NoContentResult>();
        await Namespace.Received(1).DeleteTopicAsync(topicName, Arg.Any<CancellationToken>());
    }
}
