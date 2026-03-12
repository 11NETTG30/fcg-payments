using Application.Interfaces.Interfaces.Repositories;
using Domain.Repositories;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class InfraDependencyInjection
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
        if (string.IsNullOrWhiteSpace(configuration["RABBITMQ_HOST"]) || 
            string.IsNullOrWhiteSpace(configuration["RABBITMQ_USER"]) || 
            string.IsNullOrWhiteSpace(configuration["RABBITMQ_PASSWORD"]))
            throw new Exception("Configuração do RabbitMQ faltando.");

        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderPlacedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(
                    configuration["RABBITMQ_HOST"],
                    "/",
                    h =>
                    {
                        h.Username(configuration["RABBITMQ_USER"]!);
                        h.Password(configuration["RABBITMQ_PASSWORD"]!);
                    });

                cfg.ConfigureEndpoints(context);
                cfg.UseMessageRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(5));
                });
            });
        });
        #endregion

        return services;
    }
}
