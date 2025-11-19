using Identity.Application.Abstractions.Security;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Infrastructure.Security;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public (string tokenPlain, string tokenHash) Generate(Guid userId)
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(64); // 512-bit
        var tokenPlain = Convert.ToBase64String(tokenBytes);

        // Hash using SHA256 (or HMAC with secret)
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(tokenPlain));
        var tokenHash = Convert.ToHexString(hash);

        return (tokenPlain, tokenHash);
    }
}
