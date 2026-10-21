using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.TablesControllerTests;

public class GetEntitiesAsyncTests : BaseTablesControllerTests
{
    [Fact]
    public async Task Should_ReturnEntities_When_QueryMatches()
    {
        var (table, filter) = (Fake.Lorem.Word(), "PartitionKey eq 'a'");
        IReadOnlyList<IDictionary<string, object?>> entities = [new Dictionary<string, object?> { ["RowKey"] = "1" }];
        Service.GetEntitiesAsync(table, filter, 50, Arg.Any<CancellationToken>()).Returns(entities);

        var result = await Sut.GetEntitiesAsync(table, filter, 50, TestContext.Current.CancellationToken);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(entities);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-10, 1)]
    [InlineData(1000, 1000)]
    [InlineData(5000, 1000)]
    public async Task Should_ClampTopBetweenOneAndOneThousand_When_TopIsOutOfRange(int top, int expected)
    {
        var table = Fake.Lorem.Word();

        await Sut.GetEntitiesAsync(table, null, top, TestContext.Current.CancellationToken);

        await Service.Received(1).GetEntitiesAsync(table, null, expected, Arg.Any<CancellationToken>());
    }
}
