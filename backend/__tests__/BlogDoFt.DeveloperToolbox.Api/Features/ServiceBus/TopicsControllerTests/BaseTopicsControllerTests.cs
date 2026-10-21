using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.TopicsControllerTests;

public abstract class BaseTopicsControllerTests
{
    protected static readonly Faker Fake = new();

    protected BaseTopicsControllerTests()
    {
        Sut = new TopicsController(Service);
    }

    protected IServiceBusService Service { get; } = Substitute.For<IServiceBusService>();

    protected TopicsController Sut { get; }
}
