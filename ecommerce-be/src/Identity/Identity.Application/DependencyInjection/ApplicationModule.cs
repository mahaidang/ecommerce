using Identity.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Identity.Application.DependencyInjection;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var asm = Assembly.GetExecutingAssembly();

        // ✅ MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(asm));

        // ✅ FluentValidation validators
        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // ✅ Pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


        return services;
    }
}
