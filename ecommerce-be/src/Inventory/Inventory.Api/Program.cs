using Inventory.Application.Interfaces;
using Inventory.Infrastructure.DependencyInjection;
using Inventory.Infrastructure.Models;
using Inventory.Infrastructure.Services;
using InventoryService.Application.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load 2 module chính
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<InventoryDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IInventoryDbContext>(sp => sp.GetRequiredService<InventoryDbContext>());


var app = builder.Build();

app.UseSwagger(
//    c =>
//{
//    c.PreSerializeFilters.Add((doc, req) =>
//    {
//        // Detect nếu request đến từ Gateway (qua port 5000)
//        var isViaGateway = req.Host.Port == 5000 ||
//                          req.Headers.ContainsKey("X-Forwarded-Prefix") ||
//                          req.Headers["Referer"].ToString().Contains(":5000");

//        if (isViaGateway)
//        {
//            // Force URL qua Gateway
//            doc.Servers = new List<OpenApiServer>
//            {
//                new OpenApiServer
//                {
//                    Url = "http://localhost:5000/api/inventory",
//                    Description = "Via Gateway"
//                }
//            };
//        }
//        else
//        {
//            // Chạy trực tiếp service
//            doc.Servers = new List<OpenApiServer>
//            {
//                new OpenApiServer
//                {
//                    Url = $"{req.Scheme}://{req.Host.Value}",
//                    Description = "Direct Access"
//                }
//            };
//        }
//    });
//}
);

app.UseSwaggerUI();


app.MapGrpcService<InventoryGrpcService>();
app.MapControllers();

app.MapGet("/health", () => Results.Ok("OK - Inventory"));

app.Run();
