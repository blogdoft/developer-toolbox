using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Impl;
using System.Text;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Secrets;

public class SecretResponseTests
{
    [Fact]
    public void Should_MapStreamContentAsUtf8_When_CreatedFromStream()
    {
        var text = new Bogus.Faker().Lorem.Sentence();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var response = SecretResponse.From(Guid.NewGuid(), "file.txt", stream);

        response.Content.ShouldBe(text);
    }

    [Fact]
    public void Should_ReturnEmptyContent_When_StreamIsNull()
    {
        var response = SecretResponse.From(Guid.NewGuid(), "file.txt", null);

        response.Content.ShouldBeEmpty();
    }

    [Fact]
    public void Should_MapIdAndFileName_When_CreatedFromStream()
    {
        var id = Guid.NewGuid();

        var response = SecretResponse.From(id, "file.txt", null);

        (response.Id, response.FileName).ShouldBe((id, "file.txt"));
    }

    [Fact]
    public void Should_MapTableFields_When_CreatedFromTable()
    {
        var table = SecretsTableFaker.Generate();
        table.Content = Encoding.UTF8.GetBytes("secret");

        var response = SecretResponse.From(table);

        (response.Id, response.FileName, response.Content).ShouldBe((table.NavigationId, table.FileName!, "secret"));
    }

    [Fact]
    public void Should_UseUndefinedFileName_When_TableFileNameIsNull()
    {
        var table = SecretsTableFaker.Generate();
        table.FileName = null;

        var response = SecretResponse.From(table);

        response.FileName.ShouldBe("undefined");
    }

    [Fact]
    public void Should_ReturnEmptyContent_When_TableContentIsNull()
    {
        var table = SecretsTableFaker.Generate();
        table.Content = null;

        var response = SecretResponse.From(table);

        response.Content.ShouldBeEmpty();
    }
}
