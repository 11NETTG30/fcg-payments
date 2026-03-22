using FCG.Shared.Contracts.Events;
using Application.Interfaces.Interfaces.Repositories;
using Application.Interfaces.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly ILogger<OrderPlacedConsumer> _logger;
    private readonly IPagamentoService _pagamentoService;
    private readonly IProcessedEventRepository _repository;

    public OrderPlacedConsumer(
        ILogger<OrderPlacedConsumer> logger,
        IPagamentoService pagamentoService,
        IProcessedEventRepository repository)
    {
        _logger = logger;
        _pagamentoService = pagamentoService;
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        try
        {
            var message = context.Message;

            _logger.LogInformation("Mensagem recebida: {Message}", message);

            await _repository.SaveAsync(message.UserId, message.GameId);

            await _pagamentoService.ProcessarAsync(message);

            _logger.LogInformation("Mensagem Processada.");
        }
        catch (DbUpdateException)
        {
            _logger.LogInformation("Mensagem com dados duplicados, será ignorada");
            return; // já processado
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro ao processar mensagem: {Message}, erro: {Ex}", context.Message, ex.Message);
        }
    }
}