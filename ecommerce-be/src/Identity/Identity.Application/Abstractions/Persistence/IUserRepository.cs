using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken ct);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
    Task<User?> FindByIdAsync(Guid userId, CancellationToken ct);
    Task<User?> GetByUsernameOrEmailAsync(string input, CancellationToken ct);
    Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken ct);

}
