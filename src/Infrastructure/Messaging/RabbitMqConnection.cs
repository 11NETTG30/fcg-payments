using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Infrastructure.Messaging;

public class RabbitMqConnection
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMqConnection(IConfiguration config)
    {
        if (string.IsNullOrWhiteSpace(config["RABBITMQ_HOST"]))
            throw new Exception("Configuração do RabbitMQ faltando.");

        _factory = new ConnectionFactory
        {
            HostName = config["RABBITMQ_HOST"]!,
            UserName = config["RABBITMQ_USER"]!,
            Password = config["RABBITMQ_PASSWORD"]!
        };
    }

    public async Task<IChannel> CreateChannelAsync()
    {
        if (_connection == null || !_connection.IsOpen)
            _connection = await _factory.CreateConnectionAsync();

        return await _connection.CreateChannelAsync();
    }
}