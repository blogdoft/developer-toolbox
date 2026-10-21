using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Impl;
using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Secrets.SecretsControllerTests;

public class QuerySecretByIdAsyncTests : BaseSecretsControllerTests
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly string _fileName = Fake.System.FileName();
    private readonly string _content = Fake.Lorem.Sentence();

    [Fact]
    public async Task Should_ReturnNotFound_When_SecretDoesNotExist()
    {
        Repository.QueryById(_id).Returns((SecretsTable?)null);

        var result = await Sut.QuerySecretByIdAsync(_id);

        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Should_ReturnPlainTextFile_When_AcceptIsTextPlain()
    {
        GivenStoredSecret();
        AcceptHeader("text/plain");

        var result = await Sut.QuerySecretByIdAsync(_id);

        var file = result.ShouldBeOfType<FileContentResult>();
        file.ContentType.ShouldBe("text/plain");
        file.FileDownloadName.ShouldBe(_fileName);
        Encoding.UTF8.GetString(file.FileContents).ShouldBe(_content);
    }

    [Fact]
    public async Task Should_ReturnSecretResponse_When_AcceptIsJson()
    {
        GivenStoredSecret();
        AcceptHeader("application/json");

        var result = await Sut.QuerySecretByIdAsync(_id);

        var response = result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<SecretResponse>();
        (response.Id, response.FileName, response.Content).ShouldBe((_id, _fileName, _content));
    }

    [Fact]
    public async Task Should_ReturnBadRequestNamingTheMediaType_When_AcceptIsNotSupported()
    {
        GivenStoredSecret();
        AcceptHeader("application/xml");

        var result = await Sut.QuerySecretByIdAsync(_id);

        var problem = result.ShouldBeOfType<BadRequestObjectResult>().Value.ShouldBeOfType<ProblemDetails>();
        problem.Detail.ShouldBe("application/xml is not supported");
    }

    private void GivenStoredSecret()
    {
        var table = SecretsTableFaker.Generate();
        Repository.QueryById(_id).Returns(table);
        Cryptography.DecryptAsync(table).Returns((_fileName, new MemoryStream(Encoding.UTF8.GetBytes(_content))));
    }
}
