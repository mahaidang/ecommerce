using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public sealed class UserRepository(IdentityDbContext db) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken ct)
        => await db.Users.AddAsync(user, ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
        => await db.Users.AnyAsync(u => u.Email == email, ct);

    public async Task<User?> FindByIdAsync(Guid userId, CancellationToken ct)
    {
        return await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

    }

    public async Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken ct)
    {
        return await db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string input, CancellationToken ct)
    {
        return await db.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u =>
            u.Username == input || u.Email == input, ct);
    }

}
