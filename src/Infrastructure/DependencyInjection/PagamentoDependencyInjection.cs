using Application.Interfaces.Interfaces.Repositories;
using Application.Interfaces.Messaging;
using Domain.Repositories;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Publishers;
using Infrastructure.Messaging.QueueSetup;
using Infrastructure.Persistence.Repositories;
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
        services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();
        #endregion

        #region Messageria
        services.AddSingleton<RabbitMqConnection>();
        services.AddScoped<IEventPublisher, RabbitMqPublisher>();

        services.AddHostedService<RabbitMqTopologySetup>();
        services.AddHostedService<OrderPlacedConsumer>();
        #endregion

        return services;
    }
}
