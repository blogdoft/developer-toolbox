using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.TablesControllerTests;

public class CreateTableAsyncTests : BaseTablesControllerTests
{
    [Fact]
    public async Task Should_CreateTableAndReturnCreated_When_CalledWithName()
    {
        var table = Fake.Lorem.Word();

        var result = await Sut.CreateTableAsync(table, CancellationToken.None);

        result.ShouldBeOfType<CreatedResult>();
        await Service.Received(1).CreateTableAsync(table, Arg.Any<CancellationToken>());
    }
}
