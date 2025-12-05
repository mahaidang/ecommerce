using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Product.Application.Abstractions.External;
using Product.Application.Abstractions.Persistence;
using Product.Infrastructure.Cloudinary;
using Product.Infrastructure.Grpc;
using Product.Infrastructure.Repositories;

namespace Product.Infrastructure.DependencyInjection;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Mongo config
        var mongoCfg = config.GetSection("Mongo");
        var connStr = mongoCfg.GetValue<string>("ConnectionString")!;
        var dbName = mongoCfg.GetValue<string>("Database")!;

        // MongoDB DI
        services.AddSingleton<IMongoClient>(_ => new MongoClient(connStr));
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));

        // Repository
        services.AddSingleton<IProductRepository, ProductRepository>();
        services.AddSingleton<ICategoryRepository, CategoryRepository>();

        services.Configure<CloudinarySettings>(
            config.GetSection("Cloudinary"));

        services.AddSingleton<ICloudinaryService>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
            return new CloudinaryService(settings);
        });

        services.AddGrpcClient<InventoryService.Grpc.InventoryService.InventoryServiceClient>(o =>
        {
            o.Address = new Uri(config["Grpc:InventoryUrl"]!);
        });

        services.AddScoped<IInventoryGrpcClient, InventoryGrpcClient>();


        return services;
    }
}
