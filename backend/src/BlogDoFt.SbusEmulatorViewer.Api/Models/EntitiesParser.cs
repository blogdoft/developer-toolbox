using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BlogDoFt.SbusEmulatorViewer.Api.Models;

public class EntitiesParser
{
    private readonly string _configPath;
    private readonly IFileProvider _fileProvider;

    private readonly Lazy<Task<IReadOnlyList<Topic>>> _lazyTopics;

    public EntitiesParser(
        IOptions<ServiceBusSettings> sbusSettings,
        IWebHostEnvironment env)
    {
        _configPath = sbusSettings.Value.EmulatorConfigFile;
        _fileProvider = env.ContentRootFileProvider;
        _lazyTopics = new Lazy<Task<IReadOnlyList<Topic>>>(LoadTopicsAsync);
    }

    public Task<IReadOnlyList<Topic>> GetTopicsAsync(CancellationToken cancellationToken = default)
        => _lazyTopics.Value;


    private async Task<IReadOnlyList<Topic>> LoadTopicsAsync()
    {
        if (!File.Exists(_configPath))
        {
            throw new FileNotFoundException(
                "Arquivo de configuração não encontrado.", _configPath);
        }

        using var stream = File.OpenRead(_configPath);
        var root = await JsonSerializer
            .DeserializeAsync<RootConfig>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var namespaces = root?.UserConfig?.Namespaces ?? [];

        var topics = namespaces
            .Where(ns => ns.Topics is not null)
            .SelectMany(ns => ns.Topics!)
            .Select(t => new Topic(
                TopicName: t.Name!,
                Subscriptions: t.Subscriptions?
                    .Select(s => s.Name!)
                    .ToArray()
                ?? []
            ))
            .ToList()
            .AsReadOnly();

        return topics;
    }


    private sealed class SubscriptionConfig
    {
        public string? Name { get; set; }
    }

    private sealed class TopicConfig
    {
        public string? Name { get; set; }
        public SubscriptionConfig[]? Subscriptions { get; set; }
    }

    private sealed class NamespaceConfig
    {
        public string? Name { get; set; }
        public TopicConfig[]? Topics { get; set; }
    }
    private sealed class UserConfig
    {
        public NamespaceConfig[]? Namespaces { get; set; }
    }

    private sealed class RootConfig
    {
        public UserConfig? UserConfig { get; set; }
    }
}
