using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class CreateTopicAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNamespaceResult_When_Called()
    {
        var topicName = Fake.Lorem.Word();
        var settings = new TopicSettings();
        var expected = new Topic("t", new TopicSettings(), []);
        Namespace.CreateTopicAsync(topicName, settings, Arg.Any<CancellationToken>()).Returns(expected);

        var result = await Sut.CreateTopicAsync(topicName, settings, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(expected);
    }
}
