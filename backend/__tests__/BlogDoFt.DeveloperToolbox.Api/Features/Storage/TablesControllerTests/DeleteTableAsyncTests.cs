using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.TablesControllerTests;

public class DeleteTableAsyncTests : BaseTablesControllerTests
{
    [Fact]
    public async Task Should_DeleteTableAndReturnNoContent_When_CalledWithName()
    {
        var table = Fake.Lorem.Word();

        var result = await Sut.DeleteTableAsync(table, CancellationToken.None);

        result.ShouldBeOfType<NoContentResult>();
        await Service.Received(1).DeleteTableAsync(table, Arg.Any<CancellationToken>());
    }
}
