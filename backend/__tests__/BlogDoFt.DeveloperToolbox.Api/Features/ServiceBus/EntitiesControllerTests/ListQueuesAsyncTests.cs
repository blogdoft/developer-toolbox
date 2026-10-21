using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class ListQueuesAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNamespaceResult_When_Called()
    {
        IReadOnlyList<Queue> expected = [];
        Namespace.GetQueuesAsync(Arg.Any<CancellationToken>()).Returns(expected);

        var result = await Sut.ListQueuesAsync(TestContext.Current.CancellationToken);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(expected);
    }
}
