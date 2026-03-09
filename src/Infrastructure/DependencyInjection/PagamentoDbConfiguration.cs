using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class PagamentoDbConfiguration
{
    public static IServiceCollection AddDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PagamentoDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "pagamento")
            ));

        return services;
    }
}
