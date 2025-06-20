using BlogDoFt.SbusEmulatorViewer.Api.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BlogDoFt.SbusEmulatorViewer.Api.Features.ServiceBus.Models;

public class EntitiesParser
{
    private readonly string _configPath;

    private readonly Lazy<Task<IReadOnlyList<Topic>>> _lazyTopics;

    public EntitiesParser(
        IOptions<ServiceBusSettings> sbusSettings)
    {
        _configPath = sbusSettings.Value.EmulatorConfigFile;
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
                PropertyNameCaseInsensitive = true,
            });

        var namespaces = root?.UserConfig?.Namespaces ?? [];

        var topics = namespaces
            .Where(ns => ns.Topics is not null)
            .SelectMany(ns => ns.Topics!)
            .Select(t => new Topic(
                TopicName: t.Name!,
                Subscriptions: t.Subscriptions?.Select(s => s.Name!).ToArray() ?? []))
            .ToList()
            .AsReadOnly();

        return topics;
    }

    private sealed record SubscriptionConfig(string? Name)
    {
    }

    private sealed record TopicConfig(string? Name, SubscriptionConfig[]? Subscriptions)
    {
    }

    private sealed record NamespaceConfig(TopicConfig[]? Topics)
    {
    }

    private sealed record UserConfig(NamespaceConfig[]? Namespaces)
    {
    }

    private sealed record RootConfig(UserConfig? UserConfig)
    {
    }
}
