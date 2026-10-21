using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class UpdateTopicAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNoContent_When_UpdateTopicSucceeds()
    {
        var topicName = Fake.Lorem.Word();
        var settings = new TopicSettings();

        var result = await Sut.UpdateTopicAsync(topicName, settings, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<NoContentResult>();
        await Namespace.Received(1).UpdateTopicAsync(topicName, settings, Arg.Any<CancellationToken>());
    }
}
