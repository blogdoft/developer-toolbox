using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.QueuesStorageControllerTests;

public class GetQueuesAsyncTests : BaseQueuesStorageControllerTests
{
    [Fact]
    public async Task Should_ReturnQueues_When_QueuesExist()
    {
        IReadOnlyList<StorageQueueSummary> queues = [new(Fake.Lorem.Word(), 3)];
        Service.GetQueuesAsync(Arg.Any<CancellationToken>()).Returns(queues);

        var result = await Sut.GetQueuesAsync(CancellationToken.None);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(queues);
    }
}
