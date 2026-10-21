using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class CreateQueueAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNamespaceResult_When_Called()
    {
        var queueName = Fake.Lorem.Word();
        var settings = new QueueSettings();
        var expected = new Queue("q", new QueueSettings());
        Namespace.CreateQueueAsync(queueName, settings, Arg.Any<CancellationToken>()).Returns(expected);

        var result = await Sut.CreateQueueAsync(queueName, settings, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(expected);
    }
}
