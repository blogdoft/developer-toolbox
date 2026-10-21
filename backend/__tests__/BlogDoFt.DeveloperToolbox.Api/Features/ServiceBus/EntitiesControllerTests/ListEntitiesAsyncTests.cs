using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public class ListEntitiesAsyncTests : BaseEntitiesControllerTests
{
    [Fact]
    public async Task Should_ReturnNamespaceResult_When_Called()
    {
        IReadOnlyList<Topic> expected = [];
        Namespace.GetTopicsAsync(Arg.Any<CancellationToken>()).Returns(expected);

        var result = await Sut.ListEntitiesAsync(TestContext.Current.CancellationToken);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(expected);
    }
}
