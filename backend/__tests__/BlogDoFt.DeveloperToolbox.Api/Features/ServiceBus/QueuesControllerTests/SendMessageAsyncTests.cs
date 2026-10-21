using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.QueuesControllerTests;

public class SendMessageAsyncTests : BaseQueuesControllerTests
{
    [Fact]
    public async Task Should_SendMessageToQueueAndReturnOk_When_CalledWithQueueName()
    {
        var queue = Fake.Lorem.Word();
        var message = new { Text = Fake.Lorem.Sentence() };

        var result = await Sut.SendMessageAsync(queue, message);

        result.ShouldBeOfType<OkResult>();
        await Service.Received(1).SendMessageAsync(queue, message);
    }
}
