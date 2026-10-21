using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.TablesControllerTests;

public class DeleteEntityAsyncTests : BaseTablesControllerTests
{
    [Fact]
    public async Task Should_DeleteEntityAndReturnNoContent_When_CalledWithKeys()
    {
        var (table, partition, row) = (Fake.Lorem.Word(), Fake.Lorem.Word(), Fake.Lorem.Word());

        var result = await Sut.DeleteEntityAsync(table, partition, row, CancellationToken.None);

        result.ShouldBeOfType<NoContentResult>();
        await Service.Received(1).DeleteEntityAsync(table, partition, row, Arg.Any<CancellationToken>());
    }
}
