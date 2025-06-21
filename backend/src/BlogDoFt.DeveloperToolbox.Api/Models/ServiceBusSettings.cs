namespace BlogDoFt.DeveloperToolbox.Api.Models;

public class ServiceBusSettings
{
    public required string ConnectionString { get; init; }

    public required string EmulatorConfigFile { get; init; }
}
