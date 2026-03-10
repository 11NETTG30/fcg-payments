using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.QueueSetup;

public class RabbitMqTopologySetup : IHostedService
{
    private readonly RabbitMqConnection _connection;
    private readonly ILogger<RabbitMqTopologySetup> _logger;

    public RabbitMqTopologySetup(
        RabbitMqConnection connection,
        ILogger<RabbitMqTopologySetup> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var channel = await _connection.CreateChannelAsync();

        _logger.LogInformation("Configurando filas do RabbitMQ...");

        // DLQ
        await channel.QueueDeclareAsync(
            queue: "order-placed.dlq",
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Retry queue
        var retryArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", "order-placed" },
            { "x-message-ttl", 5000 }
        };

        await channel.QueueDeclareAsync(
            queue: "order-placed.retry",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: retryArgs);

        // Main queue
        var mainArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", "order-placed.retry" }
        };

        await channel.QueueDeclareAsync(
            queue: "order-placed",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: mainArgs);

        _logger.LogInformation("Filas do RabbitMQ configuradas.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}