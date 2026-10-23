using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.Application.Abstraction;
using Pos.Infrastructure.Data;
using Pos.Infrastructure.Data.Repositories;

namespace Pos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        var connectionString = configuration.GetConnectionString("PosDb") ?? throw new InvalidOperationException("Connection string is not configured.");
        services.AddDbContext<PosDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Add repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        
        return services;
    }
}
