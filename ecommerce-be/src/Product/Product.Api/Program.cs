using Microsoft.OpenApi.Models;
using Product.Application.DependencyInjection;
using Product.Infrastructure.DependencyInjection;
using Shared.Infrastructure.Auth;


var builder = WebApplication.CreateBuilder(args);

var jwtConfigPath = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath,
        "..", "..", "..",
        "jwtsettings.dev.json")
);


builder.Configuration.AddJsonFile(jwtConfigPath, optional: false, reloadOnChange: true);

// 2) Add Authentication (BẮT BUỘC)
// =======================================================
builder.Services.AddAuthentication("Bearer");

// =======================================================
// 3) Add Shared Auth (validate JWT từ Identity)
// =======================================================
builder.Services.AddSharedAuth(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new() { Title = "Order API", Version = "v1" });

    o.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập: Bearer {token}"
    });

    o.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Gọi module
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((doc, req) =>
    {
        // Detect nếu request đến từ Gateway (qua port 5000)
        var isViaGateway = req.Host.Port == 5000 ||
                          req.Headers.ContainsKey("X-Forwarded-Prefix") ||
                          req.Headers["Referer"].ToString().Contains(":5000");

        if (isViaGateway)
        {
            // Force URL qua Gateway
            doc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = "http://localhost:5000/api/product",
                    Description = "Via Gateway"
                }
            };
        }
        else
        {
            // Chạy trực tiếp service
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
app.UseAuthentication();   // 🔥 BẮT BUỘC
app.UseAuthorization();    // 🔥 BẮT BUỘC

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok("OK"));

app.MapControllers();


app.Run();