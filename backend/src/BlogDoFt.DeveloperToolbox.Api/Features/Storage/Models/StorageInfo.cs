namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage.Models;

/// <summary>
/// Where the storage services are connected to.
/// </summary>
public record StorageInfo(bool IsEmulator, string AccountName);
