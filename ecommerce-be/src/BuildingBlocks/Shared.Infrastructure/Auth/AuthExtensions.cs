using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Abstractions.Auth;

namespace Shared.Infrastructure.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddSharedAuth(this IServiceCollection services, IConfiguration config)
    {
        // Read from "Jwt" section
        var jwtSection = config.GetSection("Jwt");
        var jwtOptions = jwtSection.Get<JwtOptions>();

        // Register options
        services.Configure<JwtOptions>(jwtSection);
        services.AddSingleton(jwtOptions);

        // JWT Bearer
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key)),

                    ValidateLifetime = true
                };
            });

        // Authorization
        services.AddAuthorization();

        // Current User resolution
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}
