using Azure.Messaging.ServiceBus;
using BlogDoFt.DeveloperToolbox.Api.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BlogDoFt.DeveloperToolbox.Api.Features.ServiceBus.Impl;

public class ServiceBusService : IServiceBusService
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSettings _settings;

    public ServiceBusService(ServiceBusClient client, IOptions<ServiceBusSettings> settings)
    {
        _client = client;
        _settings = settings.Value;
    }

    public async Task SendMessageAsync(string entityName, object message)
    {
        // Cria um sender para a fila
        await using var sender = _client.CreateSender(entityName);
        await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(message)));
    }

    public async Task<IEnumerable<object>> TopicReceiveMessagesAsync(
        string entityName,
        string subscription,
        int maxMessages = 10,
        CancellationToken cancellation = default)
    {
        await using var client = new ServiceBusClient(_settings.ConnectionString);
        await using var receiver = client.CreateReceiver(
            entityName,
            subscription,
            new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
            });

        var messages =
            await receiver.ReceiveMessagesAsync(
                maxMessages,
                maxWaitTime: TimeSpan.FromSeconds(5),
                cancellationToken: cancellation);

        var list = new List<object>();
        foreach (var msg in messages)
        {
            var msgDetails = new
            {
                Details = msg,
                Body = JsonSerializer.Deserialize<object>(msg.Body.ToString()),
            };
            list.Add(msgDetails);

            await receiver.CompleteMessageAsync(msg, cancellation);
        }

        return list;
    }
}
