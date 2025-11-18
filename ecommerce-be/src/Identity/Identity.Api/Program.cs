using Identity.Application.Abstractions.Persistence;
using Identity.Application.DependencyInjection;
using Identity.Application.Features.Commands.Auth;
using Identity.Infrastructure.DependencyInjection;
using Identity.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var jwtConfigPath = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath,
        "..", "..", "..",
        "jwtsettings.dev.json")
);


builder.Configuration.AddJsonFile(jwtConfigPath, optional: false, reloadOnChange: true);

// Load 2 module chính
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddRouting(options => options.LowercaseUrls = true);


// OpenAPI + Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    // hiển thị nút Authorize (Bearer)
    o.SwaggerDoc("v1", new() { Title = "Identity API", Version = "v1" });

    o.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập: Bearer {token}"
    });
    o.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// EF
builder.Services.AddDbContext<IdentityDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterHandler>());

// JWT
var jwt = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
        {
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((doc, req) =>
        {
            var isViaGateway = req.Host.Port == 5000 ||
                               req.Headers.ContainsKey("X-Forwarded-Prefix") ||
                               req.Headers["Referer"].ToString().Contains(":5000");

            if (isViaGateway)
            {
                doc.Servers = new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Url = "http://localhost:5000/api/identity",
                        Description = "Via Gateway"
                    }
                };
            }
            else
            {
                doc.Servers = new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Url = $"{req.Scheme}://{req.Host.Value}",
                        Description = "Direct Access"
                    }
                };
            }
        });
    });

    app.UseSwaggerUI();
//}



//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok("OK"));


app.MapControllers();

app.Run();