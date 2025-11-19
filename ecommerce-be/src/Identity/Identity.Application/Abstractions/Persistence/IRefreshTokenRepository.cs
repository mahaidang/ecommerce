using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Persistencel;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct);
    Task<IEnumerable<RefreshToken>> GetActiveByUserAsync(Guid userId, CancellationToken ct);
    Task SaveChangeAsync(CancellationToken ct);
}
