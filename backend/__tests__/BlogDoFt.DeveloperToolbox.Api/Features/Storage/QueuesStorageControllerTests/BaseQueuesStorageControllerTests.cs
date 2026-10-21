using BlogDoFt.DeveloperToolbox.Api.Features.Storage;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.Storage.QueuesStorageControllerTests;

public abstract class BaseQueuesStorageControllerTests
{
    protected static readonly Faker Fake = new();

    protected BaseQueuesStorageControllerTests()
    {
        Sut = new QueuesStorageController(Service);
    }

    protected IQueueStorageService Service { get; } = Substitute.For<IQueueStorageService>();

    protected QueuesStorageController Sut { get; }
}
