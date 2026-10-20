namespace BlogDoFt.DeveloperToolbox.Api.Models;

public class ServiceBusSettings
{
    public required string ConnectionString { get; init; }

    /// <summary>
    /// Path of the emulator config file. Only needed when connected to the emulator.
    /// </summary>
    public string? EmulatorConfigFile { get; init; }

    /// <summary>
    /// Whether the connection string targets the Service Bus emulator.
    /// </summary>
    public bool IsEmulator => ConnectionString
        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Any(part => part.Equals("UseDevelopmentEmulator=true", StringComparison.OrdinalIgnoreCase));
}
