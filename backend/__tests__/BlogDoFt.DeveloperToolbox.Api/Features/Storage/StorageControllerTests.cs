using Azure.Storage.Blobs;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using BlogDoFt.DeveloperToolbox.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage;

public class StorageControllerTests
{
    private readonly StorageController _sut = new();
    private readonly BlobServiceClient _client = Substitute.For<BlobServiceClient>();

    [Theory]
    [InlineData("UseDevelopmentStorage=true", true)]
    [InlineData("DefaultEndpointsProtocol=https;AccountName=acme;AccountKey=abc", false)]
    public void Should_ReportEmulatorFlagAndAccountName_When_Called(string connectionString, bool isEmulator)
    {
        var accountName = new Bogus.Faker().Random.AlphaNumeric(10).ToLowerInvariant();
        _client.AccountName.Returns(accountName);
        var settings = Options.Create(new StorageSettings { ConnectionString = connectionString });

        var result = _sut.GetInfo(settings, _client);

        result.ShouldBeOfType<OkObjectResult>().Value.ShouldBe(new StorageInfo(isEmulator, accountName));
    }
}
