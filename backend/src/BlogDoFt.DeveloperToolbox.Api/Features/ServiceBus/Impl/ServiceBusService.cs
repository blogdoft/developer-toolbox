using Azure.Messaging.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Impl;

public class ServiceBusService : IServiceBusService
{
    private const int PurgeBatchSize = 100;

    private readonly ServiceBusClient _client;
    private readonly ServiceBusSettings _settings;

    public ServiceBusService(ServiceBusClient client, IOptions<ServiceBusSettings> settings)
    {
        _client = client;
        _settings = settings.Value;
    }

    public async Task SendMessageAsync(string entityName, object message)
    {
        await using var sender = _client.CreateSender(entityName);
        await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(message)));
    }

    public async Task<IEnumerable<object>> PeekMessagesAsync(
        string entityName,
        string? subscription,
        int maxMessages = 10,
        CancellationToken cancellation = default)
    {
        await using var client = new ServiceBusClient(_settings.ConnectionString);
        await using var receiver = CreateReceiver(client, entityName, subscription, ServiceBusReceiveMode.PeekLock);

        var messages = await receiver.PeekMessagesAsync(maxMessages, cancellationToken: cancellation);
        return messages.Select(ToDetails).ToList();
    }

    public async Task<IEnumerable<object>> ReceiveMessagesAsync(
        string entityName,
        string? subscription,
        int maxMessages = 10,
        CancellationToken cancellation = default)
    {
        await using var client = new ServiceBusClient(_settings.ConnectionString);
        await using var receiver = CreateReceiver(client, entityName, subscription, ServiceBusReceiveMode.PeekLock);

        var messages =
            await receiver.ReceiveMessagesAsync(
                maxMessages,
                maxWaitTime: TimeSpan.FromSeconds(5),
                cancellationToken: cancellation);

        var list = new List<object>();
        foreach (var msg in messages)
        {
            list.Add(ToDetails(msg));
            await receiver.CompleteMessageAsync(msg, cancellation);
        }

        return list;
    }

    public async Task<int> PurgeMessagesAsync(
        string entityName,
        string? subscription,
        CancellationToken cancellation = default)
    {
        await using var client = new ServiceBusClient(_settings.ConnectionString);
        await using var receiver = CreateReceiver(client, entityName, subscription, ServiceBusReceiveMode.ReceiveAndDelete);

        var purged = 0;
        while (true)
        {
            var batch = await receiver.ReceiveMessagesAsync(
                PurgeBatchSize,
                maxWaitTime: TimeSpan.FromSeconds(2),
                cancellationToken: cancellation);
            if (batch.Count == 0)
            {
                return purged;
            }

            purged += batch.Count;
        }
    }

    private static ServiceBusReceiver CreateReceiver(
        ServiceBusClient client,
        string entityName,
        string? subscription,
        ServiceBusReceiveMode mode)
    {
        var options = new ServiceBusReceiverOptions { ReceiveMode = mode };
        return subscription is null
            ? client.CreateReceiver(entityName, options)
            : client.CreateReceiver(entityName, subscription, options);
    }

    private static object ToDetails(ServiceBusReceivedMessage msg)
    {
        var text = msg.Body.ToString();
        object? body;
        try
        {
            body = JsonSerializer.Deserialize<object>(text);
        }
        catch (JsonException)
        {
            body = text;
        }

        return new { Details = msg, Body = body };
    }
}
