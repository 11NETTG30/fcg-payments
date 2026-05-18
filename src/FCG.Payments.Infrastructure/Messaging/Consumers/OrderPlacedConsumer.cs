using FCG.Shared.Contracts.Events;
using Application.Interfaces.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly ILogger<OrderPlacedConsumer> _logger;
    private readonly IPagamentoService _pagamentoService;

    public OrderPlacedConsumer(
        ILogger<OrderPlacedConsumer> logger,
        IPagamentoService pagamentoService)
    {
        _logger = logger;
        _pagamentoService = pagamentoService;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        try
        {
            var message = context.Message;

            _logger.LogInformation("Mensagem recebida: {Message}", message);

            await _pagamentoService.ProcessarAsync(message);

            _logger.LogInformation("Mensagem Processada.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro ao processar mensagem: {Message}, erro: {Ex}", context.Message, ex.Message);
        }
    }
}
