using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.TablesControllerTests;

public class UpsertEntityAsyncTests : BaseTablesControllerTests
{
    [Fact]
    public async Task Should_UpsertEntityAndReturnNoContent_When_KeysArePresent()
    {
        var table = Fake.Lorem.Word();
        var entity = new Dictionary<string, JsonElement>
        {
            ["PartitionKey"] = Json("p"),
            ["RowKey"] = Json("r"),
        };

        var result = await Sut.UpsertEntityAsync(table, entity, CancellationToken.None);

        result.ShouldBeOfType<NoContentResult>();
        await Service.Received(1).UpsertEntityAsync(
            table,
            Arg.Is<IDictionary<string, object?>>(e => e.Count == 2 && e.ContainsKey("PartitionKey") && e.ContainsKey("RowKey")),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("PartitionKey")]
    [InlineData("RowKey")]
    public async Task Should_ReturnBadRequestAndSkipUpsert_When_AKeyIsMissing(string presentKey)
    {
        var entity = new Dictionary<string, JsonElement> { [presentKey] = Json("x") };

        var result = await Sut.UpsertEntityAsync(Fake.Lorem.Word(), entity, TestContext.Current.CancellationToken);

        var problem = result.ShouldBeOfType<ObjectResult>();
        problem.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        await Service.DidNotReceiveWithAnyArgs().UpsertEntityAsync(default!, default!, TestContext.Current.CancellationToken);
    }
}
