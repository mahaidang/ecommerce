using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;
using Shared.Contracts.RoutingKeys;

namespace Identity.Application.Features.Commands.Admin;

public record ApproveOrderCommand(Guid OrderId, bool Approved, string? note) : IRequest<Unit>;

public class ApproveOrderHandler : IRequestHandler<ApproveOrderCommand, Unit>
{
    private readonly IPublishEndpoint _publisher;
    private readonly ILogger<ApproveOrderHandler> _logger;

    public ApproveOrderHandler(IPublishEndpoint publisher, ILogger<ApproveOrderHandler> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Unit> Handle(ApproveOrderCommand request, CancellationToken ct)
    {
        var evt = new EventEnvelope<OrderApprovalResult>(
            request.Approved ? Rk.AdminApproved : Rk.AdminRejected,
            Guid.NewGuid(),
            request.OrderId,
            new OrderApprovalResult(request.Approved, request.note),
            DateTime.UtcNow
        );
        await _publisher.Publish(evt);

        _logger.LogInformation(
            "✅ Admin approval sent for Order {OrderId}, Approved: {Approved}",
            request.OrderId, request.Approved
        );

        return Unit.Value;
    }
}