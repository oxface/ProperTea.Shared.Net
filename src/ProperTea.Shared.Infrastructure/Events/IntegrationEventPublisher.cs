using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using ProperTea.Domain.Shared.Events;

namespace ProperTea.Infrastructure.Shared.Events;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent;
}

public class ServiceBusIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly ILogger<ServiceBusIntegrationEventPublisher> logger;
    private readonly ServiceBusSender sender;

    public ServiceBusIntegrationEventPublisher(ServiceBusSender sender,
        ILogger<ServiceBusIntegrationEventPublisher> logger)
    {
        this.sender = sender;
        this.logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent
    {
        try
        {
            var messageBody = JsonSerializer.Serialize(@event);
            var message = new ServiceBusMessage(messageBody)
            {
                Subject = typeof(TEvent).Name,
                MessageId = @event.Id.ToString(),
                CorrelationId = @event.Id.ToString()
            };

            await sender.SendMessageAsync(message, cancellationToken);
            logger.LogInformation("Published integration event {EventType} with ID {EventId}",
                typeof(TEvent).Name, @event.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish integration event {EventType} with ID {EventId}",
                typeof(TEvent).Name, @event.Id);
            throw;
        }
    }
}