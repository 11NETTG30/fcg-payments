using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Messaging.Consumers;

public class OrderPlacedConsumer : BackgroundService
{
    private readonly RabbitMqConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(
        RabbitMqConnection connection,
        IServiceScopeFactory scopeFactory,
        ILogger<OrderPlacedConsumer> logger)
    {
        _connection = connection;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = await _connection.CreateChannelAsync();

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            var body = args.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            try
            {
                var message = JsonSerializer.Deserialize<OrderPlacedEvent>(json);

                // Idempotência
                using var processedEventScope = _scopeFactory.CreateScope();
                var processedEventRepository = processedEventScope.ServiceProvider
                    .GetRequiredService<IProcessedEventRepository>();

                if (await processedEventRepository.ExistsAsync(message.OrderId))
                {
                    await channel.BasicAckAsync(args.DeliveryTag, false);
                    return;
                }

                using var scope = _scopeFactory.CreateScope();

                var service = scope.ServiceProvider
                    .GetRequiredService<IPagamentoService>();

                if (message != null)
                    await service.ProcessarAsync(message);

                await processedEventRepository.SaveAsync(message.OrderId);
                await channel.BasicAckAsync(args.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao processar mensagem: {Message}, erro: {Ex}", json, ex.Message);

                if (ExcedeuRetries(args, 3))
                {
                    await channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: "order-placed.dlq",
                        body: args.Body);

                    await channel.BasicAckAsync(args.DeliveryTag, false);
                }
                else
                    await channel.BasicNackAsync(args.DeliveryTag, false, false);
            }
        };

        await channel.BasicConsumeAsync(
            queue: "order-placed",
            autoAck: false,
            consumerTag: "",
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: consumer);

        //Para manter o consumer vivo
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private static bool ExcedeuRetries(BasicDeliverEventArgs args, int maxRetries)
    {
        if (args.BasicProperties.Headers == null)
            return false;

        if (!args.BasicProperties.Headers.ContainsKey("x-death"))
            return false;

        var deaths = (List<object>)args.BasicProperties.Headers["x-death"];

        var death = (Dictionary<string, object>)deaths[0];

        var count = (long)death["count"];

        return count >= maxRetries;
    }
}