using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Impl;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Secrets;

public class SecretLookupResponseTests
{
    [Fact]
    public void Should_MapNavigationIdAndFileName_When_CreatedFromTable()
    {
        var table = SecretsTableFaker.Generate();

        var response = SecretLookupResponse.From(table);

        (response.Id, response.FileName).ShouldBe((table.NavigationId, table.FileName!));
    }

    [Fact]
    public void Should_UseUndefinedFileName_When_TableFileNameIsNull()
    {
        var table = SecretsTableFaker.Generate();
        table.FileName = null;

        var response = SecretLookupResponse.From(table);

        response.FileName.ShouldBe("undefined");
    }
}
