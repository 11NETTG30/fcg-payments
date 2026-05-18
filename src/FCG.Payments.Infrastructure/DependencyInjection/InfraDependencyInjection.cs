using Domain.Repositories;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Setup;
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
        #endregion

        #region Messageria
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));

        var rabbit = configuration
                .GetSection("RabbitMQ")
                .Get<RabbitMqSettings>()!;

        if (string.IsNullOrWhiteSpace(rabbit.Host) || 
            string.IsNullOrWhiteSpace(rabbit.Username) || 
            string.IsNullOrWhiteSpace(rabbit.Password))
            throw new Exception("Configuração do RabbitMQ faltando.");

        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderPlacedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.MessageTopology.SetEntityNameFormatter(new CustomNameEntityNameFormatter());

                cfg.Host(
                    rabbit.Host,
                    rabbit.VirtualHost,
                    h =>
                    {
                        h.Username(rabbit.Username);
                        h.Password(rabbit.Password);
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
