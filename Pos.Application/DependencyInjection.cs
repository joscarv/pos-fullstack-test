using Microsoft.Extensions.DependencyInjection;
using Pos.Application.Abstraction;
using Pos.Application.Services;

namespace Pos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
