using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace InventoryService.Application.DependencyInjection;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ✅ MediatR – tự động scan tất cả các Command/Handler trong Assembly này
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        //// ✅ AutoMapper (nếu bạn có mapping giữa DTO ↔ Entity)
        //services.AddAutoMapper(Assembly.GetExecutingAssembly());

        //// ✅ FluentValidation – tự động tìm tất cả Validator<>
        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        //// ✅ Pipeline Behavior (tuỳ chọn, nếu dùng validation hoặc logging pipeline)
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
