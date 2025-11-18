using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Report.Application.Abstractions.Persistence;
using Report.Application.Features.Queries;
using ReportService.Application;
using ReportService.Infrastructure;
using ReportService.Infrastructure.Persistence;
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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddDbContext<ReportDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IReportDbContext>(sp => sp.GetRequiredService<ReportDbContext>());

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetOrderStatusCountsQuery).Assembly));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

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
                        Url = "http://localhost:5000/api/report",
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
app.UseAuthentication();   // 🔥 BẮT BUỘC
app.UseAuthorization();    // 🔥 BẮT BUỘC
app.MapControllers();
app.MapGet("/health", () => Results.Ok("OK - ReportService"));

app.Run();
