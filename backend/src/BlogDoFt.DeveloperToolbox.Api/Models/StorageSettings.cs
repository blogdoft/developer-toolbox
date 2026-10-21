namespace BlogDoFt.DeveloperToolbox.Api.Models;

public class StorageSettings
{
    /// <summary>
    /// Azurite is used when the connection string is the development storage shortcut
    /// (or targets the well-known <c>devstoreaccount1</c> account). Any other value is
    /// handled as a real Azure Storage account.
    /// </summary>
    public string ConnectionString { get; init; } = "UseDevelopmentStorage=true";

    /// <summary>
    /// Whether the connection string targets Azurite.
    /// </summary>
    public bool IsEmulator => ConnectionString
        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Any(part =>
            part.Equals("UseDevelopmentStorage=true", StringComparison.OrdinalIgnoreCase) ||
            part.Equals("AccountName=devstoreaccount1", StringComparison.OrdinalIgnoreCase));
}
