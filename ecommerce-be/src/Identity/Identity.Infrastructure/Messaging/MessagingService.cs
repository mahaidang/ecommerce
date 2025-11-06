using Identity.Application.Abstractions.Messaging;
using MassTransit;

namespace Identity.Infrastructure.Messaging;

public class MessagingService : IMessagingService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MessagingService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<T>(T message, string routingKey, CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(message, ctx => ctx.SetRoutingKey(routingKey), ct);
    }
}