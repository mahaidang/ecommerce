using Identity.Application.Abstractions.Persistencel;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Application.Features.Commands.Auth;

public record RevokeTokenCommand(string RefreshToken) : IRequest<Unit>;

public class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand, Unit>
{
    private readonly IRefreshTokenRepository _repo;

    public RevokeTokenHandler(IRefreshTokenRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(RevokeTokenCommand req, CancellationToken ct)
    {
        // hash refresh token từ client gửi lên
        using var sha = SHA256.Create();
        var hash = Convert.ToHexString(
            sha.ComputeHash(Encoding.UTF8.GetBytes(req.RefreshToken))
        );

        var token = await _repo.GetByTokenHashAsync(hash, ct);

        if (token is not null && token.RevokedAtUtc is null)
        {
            token.RevokedAtUtc = DateTime.UtcNow;
            await _repo.SaveChangeAsync(ct);
        }

        return Unit.Value;
    }
}