using Application.Interfaces.Services;
using Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly);

        #region Service
        services.AddScoped<IPagamentoService, PagamentoService>();
        #endregion

        return services;
    }
}
