using BlogDoFt.DeveloperToolbox.Api.Models;

namespace BlogDoFt.DeveloperToolbox.Api.Tests.Models;

public class StorageSettingsTests
{
    [Fact]
    public void Should_BeEmulator_When_UsingDefaultConnectionString()
    {
        var settings = new StorageSettings();

        settings.IsEmulator.ShouldBeTrue();
    }

    [Theory]
    [InlineData("UseDevelopmentStorage=true")]
    [InlineData("usedevelopmentstorage=TRUE;DevelopmentStorageProxyUri=http://host")]
    [InlineData("DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=abc")]
    public void Should_BeEmulator_When_ConnectionStringTargetsAzurite(string connectionString)
    {
        var settings = new StorageSettings { ConnectionString = connectionString };

        settings.IsEmulator.ShouldBeTrue();
    }

    [Theory]
    [InlineData("DefaultEndpointsProtocol=https;AccountName=acme;AccountKey=abc;EndpointSuffix=core.windows.net")]
    [InlineData("UseDevelopmentStorage=false")]
    public void Should_NotBeEmulator_When_ConnectionStringTargetsRealAccount(string connectionString)
    {
        var settings = new StorageSettings { ConnectionString = connectionString };

        settings.IsEmulator.ShouldBeFalse();
    }
}
