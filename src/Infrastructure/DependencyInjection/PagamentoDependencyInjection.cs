using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class PagamentoDependencyInjection
{
    public static IServiceCollection AddDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        #region Repository
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        #endregion

        return services;
    }
}
