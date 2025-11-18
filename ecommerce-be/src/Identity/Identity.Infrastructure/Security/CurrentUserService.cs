//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using Identity.Application.Abstractions.Security;
//using Microsoft.AspNetCore.Http;

//namespace Identity.Infrastructure.Security;

//public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
//{
//    public Guid? UserId
//    {
//        get
//        {
//            var idClaim = accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
//                ?? accessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

//            return Guid.TryParse(idClaim, out var id) ? id : null;
//        }
//    }
//}
