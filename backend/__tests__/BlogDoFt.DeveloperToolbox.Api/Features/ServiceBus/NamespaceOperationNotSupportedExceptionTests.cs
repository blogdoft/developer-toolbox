using BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Features.ServiceBus;

public class NamespaceOperationNotSupportedExceptionTests
{
    [Fact]
    public void Should_MentionOperationAndEmulator_When_Created()
    {
        var operation = new Bogus.Faker().Lorem.Word();

        var exception = new NamespaceOperationNotSupportedException(operation);

        exception.Message.ShouldContain($"'{operation}'");
        exception.Message.ShouldContain("emulator");
    }
}
