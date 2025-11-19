using Identity.Application.Abstractions.Persistencel;
using MediatR;

namespace Identity.Application.Features.Commands.Auth;

public sealed record RevokeAllTokensCommand(Guid UserId) : IRequest<Unit>;


public class RevokeAllTokensHandler : IRequestHandler<RevokeAllTokensCommand, Unit>
{
    private readonly IRefreshTokenRepository _repo;

    public RevokeAllTokensHandler(IRefreshTokenRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(RevokeAllTokensCommand req, CancellationToken ct)
    {
        var tokens = await _repo.GetActiveByUserAsync(req.UserId, ct);

        foreach (var t in tokens)
            t.RevokedAtUtc = DateTime.UtcNow;

        await _repo.SaveChangeAsync(ct);

        return Unit.Value;
    }
}
