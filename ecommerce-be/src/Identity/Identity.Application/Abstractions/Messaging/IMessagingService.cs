namespace Identity.Application.Abstractions.Messaging;

public interface IMessagingService
{
    Task PublishAsync<T>(T message, string routingKey, CancellationToken ct = default);
}
