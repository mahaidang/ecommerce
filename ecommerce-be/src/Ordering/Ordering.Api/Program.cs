using Microsoft.OpenApi.Models;
using Ordering.Application.DependencyInjection;
using Ordering.Infrastructure.DependencyInjection;
using Shared.Infrastructure.Auth;


var builder = WebApplication.CreateBuilder(args);

var isRunningInDocker =
    string.Equals(
        Environment.GetEnvironmentVariable("RUNNING_IN_DOCKER"),
        "true",
        StringComparison.OrdinalIgnoreCase
    );

if (builder.Environment.IsDevelopment() &&
    string.IsNullOrEmpty(builder.Configuration["Jwt:Key"]) &&
    !isRunningInDocker)
{
    var jwtFilePath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "..", "..", "..",
        "jwtsettings.dev.json"
    );

    if (File.Exists(jwtFilePath))
    {
        builder.Configuration.AddJsonFile(jwtFilePath, optional: true, reloadOnChange: true);
    }
}




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


// Load 2 module chính
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

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
            doc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = "http://localhost:5000/api/ordering",
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
}); app.UseSwaggerUI();

app.UseAuthentication();   // 🔥 BẮT BUỘC
app.UseAuthorization();    // 🔥 BẮT BUỘC

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/health", () => Results.Ok("OK - Ordering"));

app.Run();