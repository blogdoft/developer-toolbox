using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;
using Bogus;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus.QueuesControllerTests;

public abstract class BaseQueuesControllerTests
{
    protected static readonly Faker Fake = new();

    protected BaseQueuesControllerTests()
    {
        Sut = new QueuesController(Service);
    }

    protected IServiceBusService Service { get; } = Substitute.For<IServiceBusService>();

    protected QueuesController Sut { get; }
}
