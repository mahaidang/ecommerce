using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public sealed class UserRepository(IdentityDbContext db) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken ct)
        => await db.Users.AddAsync(user, ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
        => db.Users.AnyAsync(u => u.Email == email, ct);

    public Task<User?> FindByIdAsync(Guid userId, CancellationToken ct)
    {
        return db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

    }

    public Task<User?> GetByUsernameOrEmailAsync(string input, CancellationToken ct)
    {
        return db.Users
            .FirstOrDefaultAsync(u => u.Username == input || u.Email == input, ct);
    }
}
