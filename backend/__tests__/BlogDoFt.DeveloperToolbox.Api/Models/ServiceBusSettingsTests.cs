using BlogDoFt.DeveloperToolbox.Api.Models;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Models;

public class ServiceBusSettingsTests
{
    [Theory]
    [InlineData("Endpoint=sb://localhost;SharedAccessKeyName=k;SharedAccessKey=v;UseDevelopmentEmulator=true;")]
    [InlineData("UseDevelopmentEmulator=true")]
    [InlineData("Endpoint=sb://localhost; usedevelopmentemulator=TRUE")]
    public void Should_BeEmulator_When_ConnectionStringHasDevelopmentEmulatorFlag(string connectionString)
    {
        var settings = new ServiceBusSettings { ConnectionString = connectionString };

        settings.IsEmulator.ShouldBeTrue();
    }

    [Theory]
    [InlineData("Endpoint=sb://acme.servicebus.windows.net/;SharedAccessKeyName=k;SharedAccessKey=v")]
    [InlineData("Endpoint=sb://localhost;UseDevelopmentEmulator=false")]
    public void Should_NotBeEmulator_When_ConnectionStringHasNoDevelopmentEmulatorFlag(string connectionString)
    {
        var settings = new ServiceBusSettings { ConnectionString = connectionString };

        settings.IsEmulator.ShouldBeFalse();
    }
}
