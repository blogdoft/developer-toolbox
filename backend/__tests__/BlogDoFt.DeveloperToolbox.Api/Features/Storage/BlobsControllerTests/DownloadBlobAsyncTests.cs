using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.BlobsControllerTests;

public class DownloadBlobAsyncTests : BaseBlobsControllerTests
{
    [Fact]
    public async Task Should_ReturnFileNamedAfterLastPathSegment_When_BlobNameHasSlashes()
    {
        var container = Fake.Lorem.Word();
        var blob = new BlobContent(new MemoryStream([1]), "text/plain");
        Service.DownloadBlobAsync(container, "folder/sub/a.txt", Arg.Any<CancellationToken>()).Returns(blob);

        var result = await Sut.DownloadBlobAsync(container, "folder/sub/a.txt", CancellationToken.None);

        var file = result.ShouldBeOfType<FileStreamResult>();
        (file.FileDownloadName, file.ContentType, file.FileStream).ShouldBe(("a.txt", "text/plain", blob.Content));
    }
}
