using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.TablesControllerTests;

public class GetTablesAsyncTests : BaseTablesControllerTests
{
    [Fact]
    public async Task Should_ReturnTableNames_When_TablesExist()
    {
        IReadOnlyList<string> tables = [Fake.Lorem.Word(), Fake.Lorem.Word()];
        Service.GetTablesAsync(Arg.Any<CancellationToken>()).Returns(tables);

        var result = await Sut.GetTablesAsync(CancellationToken.None);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(tables);
    }
}
