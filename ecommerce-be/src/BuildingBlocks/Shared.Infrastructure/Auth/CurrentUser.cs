using Shared.Abstractions.Auth;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace Shared.Infrastructure.Auth;

public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid? UserId
    {
        get
        {
            var idClaim = accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? accessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return Guid.TryParse(idClaim, out var id) ? id : null;
        }
    }
}
