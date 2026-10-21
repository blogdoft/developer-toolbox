using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.QueuesControllerTests;

public class PeekMessagesAsyncTests : BaseQueuesControllerTests
{
    [Fact]
    public async Task Should_ReturnPeekedMessages_When_CalledWithQueueName()
    {
        var queue = Fake.Lorem.Word();
        IEnumerable<object> messages = [new { A = 1 }];
        Service.PeekMessagesAsync(queue, null, 5, Arg.Any<CancellationToken>()).Returns(messages);

        var result = await Sut.PeekMessagesAsync(queue, 5, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(messages);
    }
}
