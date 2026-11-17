using Comercial360.Application.Ports.Out;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Comercial360.Adapters.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Comercial360")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:Comercial360.");
        services.AddDbContext<Comercial360DbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ISalesRepository, EfSalesRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        return services;
    }
}
