using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Persistencel;
using Identity.Application.Abstractions.Security;
using Identity.Domain.Entities;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Application.Features.Commands.Auth;

public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResult>;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResult>
{
    private readonly IRefreshTokenRepository _rtRepo;
    private readonly IRefreshTokenGenerator _rtGen;
    private readonly IUserRepository _users;
    private readonly IJwtTokenGenerator _jwt;

    public RefreshTokenHandler(
        IRefreshTokenRepository rtRepo,
        IRefreshTokenGenerator rtGen,
        IUserRepository users,
        IJwtTokenGenerator jwt)
    {
        _rtRepo = rtRepo;
        _rtGen = rtGen;
        _users = users;
        _jwt = jwt;
    }

    public async Task<LoginResult> Handle(RefreshTokenCommand req, CancellationToken ct)
    {
        // 1) compute hash of incoming token
        using var sha = SHA256.Create();
        var hash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(req.RefreshToken)));

        var existing = await _rtRepo.GetByTokenHashAsync(hash, ct);
        if (existing == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        // 2) load user with roles
        var user = await _users.GetByIdWithRolesAsync(existing.UserId, ct);
        if (user == null) throw new UnauthorizedAccessException("Invalid token (user)");

        // 3) revoke existing token (rotation)
        existing.RevokedAtUtc = DateTime.UtcNow;

        // 4) generate new refresh token
        var (newPlain, newHash) = _rtGen.Generate(user.Id);
        var newToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(14)
        };

        await _rtRepo.AddAsync(newToken, ct);

        // 5) generate new access token
        var accessToken = _jwt.GenerateToken(user);

        await _rtRepo.SaveChangeAsync(ct);

        return new LoginResult(user.Id, user.Username, user.Email, accessToken, newPlain);
    }
}
