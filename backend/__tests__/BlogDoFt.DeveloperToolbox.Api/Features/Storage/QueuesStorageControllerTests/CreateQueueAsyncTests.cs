using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.QueuesStorageControllerTests;

public class CreateQueueAsyncTests : BaseQueuesStorageControllerTests
{
    [Fact]
    public async Task Should_CreateQueueAndReturnCreated_When_CalledWithName()
    {
        var queue = Fake.Lorem.Word();

        var result = await Sut.CreateQueueAsync(queue, CancellationToken.None);

        result.ShouldBeOfType<CreatedResult>();
        await Service.Received(1).CreateQueueAsync(queue, Arg.Any<CancellationToken>());
    }
}
