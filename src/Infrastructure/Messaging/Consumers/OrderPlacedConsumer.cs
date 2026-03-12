using Application.Events;
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

            await _repository.SaveAsync(message.UserId, message.GameId);

            await _pagamentoService.ProcessarAsync(message);
        }
        catch (DbUpdateException)
        {
            return; // já processado
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro ao processar mensagem: {Message}, erro: {Ex}", context.Message, ex.Message);
        }
    }
}