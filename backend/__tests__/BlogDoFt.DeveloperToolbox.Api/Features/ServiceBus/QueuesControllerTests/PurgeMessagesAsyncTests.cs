using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.QueuesControllerTests;

public class PurgeMessagesAsyncTests : BaseQueuesControllerTests
{
    [Fact]
    public async Task Should_ReturnPurgedCount_When_QueueIsPurged()
    {
        var queue = Fake.Lorem.Word();
        var purged = Fake.Random.Int(1, 100);
        Service.PurgeMessagesAsync(queue, null, Arg.Any<CancellationToken>()).Returns(purged);

        var result = await Sut.PurgeMessagesAsync(queue, TestContext.Current.CancellationToken);

        var value = result.ShouldBeOfType<OkObjectResult>().Value.ShouldNotBeNull();
        value.GetType().GetProperty("Purged")!.GetValue(value).ShouldBe(purged);
    }
}
