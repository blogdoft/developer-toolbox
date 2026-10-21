using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.BlobsControllerTests;

public class UploadBlobAsyncTests : BaseBlobsControllerTests
{
    private readonly IFormFile _file = Substitute.For<IFormFile>();

    public UploadBlobAsyncTests()
    {
        _file.FileName.Returns("report.pdf");
        _file.ContentType.Returns("application/pdf");
        _file.OpenReadStream().Returns(_ => new MemoryStream([1, 2, 3]));
    }

    [Fact]
    public async Task Should_NameBlobAfterFile_When_NameIsNotProvided()
    {
        var container = Fake.Lorem.Word();

        await Sut.UploadBlobAsync(container, _file, null, CancellationToken.None);

        await Service.Received(1).UploadBlobAsync(
            container, "report.pdf", Arg.Any<Stream>(), "application/pdf", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_NameBlobAfterGivenName_When_NameIsProvided()
    {
        var (container, name) = (Fake.Lorem.Word(), "folder/custom.pdf");

        await Sut.UploadBlobAsync(container, _file, name, CancellationToken.None);

        await Service.Received(1).UploadBlobAsync(
            container, name, Arg.Any<Stream>(), "application/pdf", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_ReturnCreated_When_BlobIsUploaded()
    {
        var result = await Sut.UploadBlobAsync(Fake.Lorem.Word(), _file, null, CancellationToken.None);

        result.ShouldBeOfType<CreatedResult>();
    }
}
