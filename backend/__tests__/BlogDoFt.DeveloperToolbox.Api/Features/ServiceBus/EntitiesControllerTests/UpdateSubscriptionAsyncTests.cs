using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class UpdateSubscriptionAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNoContent_When_UpdateSubscriptionSucceeds()
    {
        var topicName = Fake.Lorem.Word();
        var subscriptionName = Fake.Lorem.Word();
        var settings = new SubscriptionSettings();

        var result = await Sut.UpdateSubscriptionAsync(topicName, subscriptionName, settings, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<NoContentResult>();
        await Namespace.Received(1).UpdateSubscriptionAsync(topicName, subscriptionName, settings, Arg.Any<CancellationToken>());
    }
}
