using BlogDoFt.DeveloperToolbox.Api.Features.Secrets.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Secrets.SecretsControllerTests;

public class SaveSecretAsyncTests : BaseSecretsControllerTests
{
    private readonly SecretSaveModel _secret = new(Fake.System.FileName(), Fake.Lorem.Sentence());
    private readonly MemoryStream _encrypted = new(Fake.Random.Bytes(32));
    private readonly byte[] _iv = Fake.Random.Bytes(16);
    private readonly Guid _id = Guid.NewGuid();

    public SaveSecretAsyncTests()
    {
        Cryptography.Encrypt(_secret).Returns((_encrypted, _iv));
        Repository.SaveAsync(_encrypted, _iv, _secret.FileName).Returns(_id);
    }

    [Fact]
    public async Task Should_ReturnCreatedAtQueryRoute_When_SecretIsSaved()
    {
        var result = await Sut.SaveSecretAsync(_secret);

        var created = result.ShouldBeOfType<CreatedAtRouteResult>();
        created.RouteName.ShouldBe("QuerySecretByIdAsync");
        created.RouteValues!["id"].ShouldBe(_id);
    }

    [Fact]
    public async Task Should_PersistEncryptedContent_When_SecretIsSaved()
    {
        await Sut.SaveSecretAsync(_secret);

        await Repository.Received(1).SaveAsync(_encrypted, _iv, _secret.FileName);
    }

    [Fact]
    public async Task Should_DisposeEncryptedStream_When_SecretIsSaved()
    {
        await Sut.SaveSecretAsync(_secret);

        _encrypted.CanRead.ShouldBeFalse();
    }
}
