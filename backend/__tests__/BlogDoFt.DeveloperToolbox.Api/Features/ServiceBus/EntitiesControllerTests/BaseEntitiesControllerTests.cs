using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.EntitiesControllerTests;

public abstract class BaseEntitiesControllerTests
{
    protected static readonly Faker Fake = new();

    protected BaseEntitiesControllerTests()
    {
        Sut = new EntitiesController(Namespace);
    }

    protected INamespaceManager Namespace { get; } = Substitute.For<INamespaceManager>();

    protected EntitiesController Sut { get; }
}
