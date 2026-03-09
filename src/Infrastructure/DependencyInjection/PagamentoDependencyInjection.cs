using Application.Services;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces.Services;

namespace Infrastructure.DependencyInjection;

public static class PagamentoDependencyInjection
{
    public static IServiceCollection AddDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        #region Repository
        services.AddScoped<IPagamentoRepository, PedidoRepository>();
        #endregion

        #region Service
        services.AddScoped<IPagamentoService, PagamentoService>();
        #endregion

        return services;
    }
}
