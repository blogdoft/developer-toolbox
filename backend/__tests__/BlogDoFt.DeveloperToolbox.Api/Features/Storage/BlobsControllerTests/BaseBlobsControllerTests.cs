using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.BlobsControllerTests;

public abstract class BaseBlobsControllerTests
{
    protected static readonly Faker Fake = new();

    protected BaseBlobsControllerTests()
    {
        Sut = new BlobsController(Service);
    }

    protected IBlobStorageService Service { get; } = Substitute.For<IBlobStorageService>();

    protected BlobsController Sut { get; }
}
