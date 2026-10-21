using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class DeleteSubscriptionAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNoContent_When_DeleteSubscriptionSucceeds()
    {
        var topicName = Fake.Lorem.Word();
        var subscriptionName = Fake.Lorem.Word();

        var result = await Sut.DeleteSubscriptionAsync(topicName, subscriptionName, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<NoContentResult>();
        await Namespace.Received(1).DeleteSubscriptionAsync(topicName, subscriptionName, Arg.Any<CancellationToken>());
    }
}
