using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Impl;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Secrets.SecretsControllerTests;

public class GetAllAsyncTests : BaseSecretsControllerTests
{
    [Fact]
    public async Task Should_ReturnOneLookupPerStoredSecret_When_SecretsExist()
    {
        var tables = SecretsTableFaker.Generate(3);
        Repository.AllAsync().Returns(tables);

        var result = await Sut.GetAllAsync();

        var items = result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeAssignableTo<IEnumerable<SecretLookupResponse>>()!;
        items.Select(i => i.Id).ShouldBe(tables.Select(t => t.NavigationId));
    }

    [Fact]
    public async Task Should_ReturnEmptyList_When_NoSecretExists()
    {
        Repository.AllAsync().Returns([]);

        var result = await Sut.GetAllAsync();

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeAssignableTo<IEnumerable<SecretLookupResponse>>()!.ShouldBeEmpty();
    }
}
