using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using BlogDoFt.DeveloperToolbox.Api.Features.Storage.Impl;
using BlogDoFt.DeveloperToolbox.Api.Models;
using Microsoft.Extensions.Options;

namespace BlogDoFt.DeveloperToolbox.Api.Features.Storage;

public static class StorageExtension
{
    /// <summary>
    /// Registers the storage services. The same clients talk to Azurite or to a real
    /// storage account, depending only on the configured connection string.
    /// </summary>
    public static IServiceCollection AddStorageFeature(this IServiceCollection services) =>
        services
            .AddSingleton(sp => new BlobServiceClient(ConnectionString(sp)))
            .AddSingleton(sp => new QueueServiceClient(
                ConnectionString(sp),
                new QueueClientOptions { MessageEncoding = QueueMessageEncoding.None }))
            .AddSingleton(sp => new TableServiceClient(ConnectionString(sp)))
            .AddSingleton<IBlobStorageService, BlobStorageService>()
            .AddSingleton<IQueueStorageService, QueueStorageService>()
            .AddSingleton<ITableStorageService, TableStorageService>();

    private static string ConnectionString(IServiceProvider sp)
        => sp.GetRequiredService<IOptions<StorageSettings>>().Value.ConnectionString;
}
