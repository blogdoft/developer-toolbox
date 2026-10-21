using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class CreateSubscriptionAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNamespaceResult_When_Called()
    {
        var topicName = Fake.Lorem.Word();
        var subscriptionName = Fake.Lorem.Word();
        var settings = new SubscriptionSettings();
        var expected = new Subscription("s", new SubscriptionSettings(), []);
        Namespace.CreateSubscriptionAsync(topicName, subscriptionName, settings, Arg.Any<CancellationToken>()).Returns(expected);

        var result = await Sut.CreateSubscriptionAsync(topicName, subscriptionName, settings, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(expected);
    }
}
