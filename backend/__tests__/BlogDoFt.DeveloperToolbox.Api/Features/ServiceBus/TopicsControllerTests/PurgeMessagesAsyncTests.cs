using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.TopicsControllerTests;

public class PurgeMessagesAsyncTests : BaseTopicsControllerTests
{
    [Fact]
    public async Task Should_ReturnPurgedCount_When_SubscriptionIsPurged()
    {
        var (topic, subscription) = (Fake.Lorem.Word(), Fake.Lorem.Word());
        var purged = Fake.Random.Int(1, 100);
        Service.PurgeMessagesAsync(topic, subscription, Arg.Any<CancellationToken>()).Returns(purged);

        var result = await Sut.PurgeMessagesAsync(topic, subscription, TestContext.Current.CancellationToken);

        var value = result.ShouldBeOfType<OkObjectResult>().Value.ShouldNotBeNull();
        value.GetType().GetProperty("Purged")!.GetValue(value).ShouldBe(purged);
    }
}
