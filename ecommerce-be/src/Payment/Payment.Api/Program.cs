using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Payment.Application.Abstractions.External;
using Payment.Application.Abstractions.Persistence;
using Payment.Application.Features.Commands;
using Payment.Infrastructure.Consumers;
using Payment.Infrastructure.External;
using Payment.Infrastructure.Models;
using Payment.Infrastructure.Repository;
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


builder.Services.AddAuthentication("Bearer");
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
builder.Services.AddDbContext<PaymentDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<CreateSePayPaymentCommand>());
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ISePayApi, SePayService>();

// ✅ Đăng ký MassTransit trực tiếp (thay toàn bộ RabbitMQ.Client thủ công)
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:User"] ?? "guest");
            h.Password(builder.Configuration["RabbitMq:Pass"] ?? "guest");
        });

        // MassTransit tự tạo exchange/queue từ các consumer
        cfg.ConfigureEndpoints(context);
    });
});


var app = builder.Build();

app.UseSwagger(
   c =>
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
                    Url = "http://localhost:5000/api/payment",
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
}
);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapGet("/health", () => Results.Ok("OK"));

app.Run();

