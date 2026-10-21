using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.TopicsControllerTests;

public class PeekMessagesAsyncTests : BaseTopicsControllerTests
{
    [Fact]
    public async Task Should_ReturnPeekedMessages_When_CalledWithSubscription()
    {
        var (topic, subscription) = (Fake.Lorem.Word(), Fake.Lorem.Word());
        IEnumerable<object> messages = [new { A = 1 }];
        Service.PeekMessagesAsync(topic, subscription, 5, Arg.Any<CancellationToken>()).Returns(messages);

        var result = await Sut.PeekMessagesAsync(topic, subscription, 5, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(messages);
    }
}
