using Identity.Application.Abstractions.Persistencel;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IdentityDbContext _db;
    public RefreshTokenRepository(IdentityDbContext db) => _db = db;

    public async Task AddAsync(RefreshToken token, CancellationToken ct)
    {
        await _db.RefreshTokens.AddAsync(token, ct);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct)
    {
        return await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenHash, ct);
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveByUserAsync(Guid userId, CancellationToken ct)
    {
        return await _db.RefreshTokens
                .Where(x => x.UserId == userId
                         && x.RevokedAtUtc == null
                         && x.ExpiresAtUtc >= DateTime.UtcNow)
                .ToListAsync(ct);
    }

    public async Task SaveChangeAsync(CancellationToken ct) => await _db.SaveChangesAsync(ct);

}

