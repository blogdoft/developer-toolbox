using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.TopicsControllerTests;

public class SendMessageAsyncTests : BaseTopicsControllerTests
{
    [Fact]
    public async Task Should_SendMessageToTopicAndReturnOk_When_CalledWithTopicName()
    {
        var topic = Fake.Lorem.Word();
        var message = new { Text = Fake.Lorem.Sentence() };

        var result = await Sut.SendMessageAsync(topic, message);

        result.ShouldBeOfType<OkResult>();
        await Service.Received(1).SendMessageAsync(topic, message);
    }
}
