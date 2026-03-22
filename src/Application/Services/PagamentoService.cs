using Application.Configuration;
using Application.DTOs;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Repositories;
using FCG.Shared.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class PagamentoService : IPagamentoService
{
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PagamentoService> _logger;

    private readonly double _taxaReprovacao;

    public PagamentoService(
        IPagamentoRepository pagamentoRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<PagamentoService> logger,
        IOptions<PagamentoOptions> options
        )
    {
        _pagamentoRepository = pagamentoRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;

        _taxaReprovacao = options.Value.TaxaReprovacao;
    }

    public async Task<List<PagamentoDto>> ListarPagamentosPorUsuarioAsync(Guid idUsuario)
    {
        var pagamentos = await _pagamentoRepository.ListarPagamentosPorUsuarioAsync(idUsuario);

        return pagamentos.Select(p => (PagamentoDto)p).ToList();
    }

    public async Task<PagamentoDto> ObterPagamentoAsync(Guid id)
    {
        var pagamento = await BuscarPagamento(id);

        return (PagamentoDto)pagamento;
    }

    public async Task<PagamentoEntity> ProcessarAsync(PagamentoRequest dadosPagamento)
    {
        await ChecarPagamentoDuplicado(dadosPagamento);

        var pagamento = await GerarPagamento(dadosPagamento);

        if (pagamento.Status != PagamentoStatus.Approved)
            throw new PagamentoRecusadoException(pagamento.Id);

        return pagamento;
    }

    public async Task ProcessarAsync(OrderPlacedEvent dadosPagamento)
    {
        _logger.LogInformation("Iniciando processamento do pagamento, usuário: {Usuario}, jogo: {Jogo}, valor: {Valor}", dadosPagamento.UserId, dadosPagamento.GameId, dadosPagamento.Price);

        var pagamento = await BuscarPagamentoDuplicadoOrderPlaced(dadosPagamento);

        if (pagamento is null)
            pagamento = await GerarPagamento((PagamentoRequest)dadosPagamento);

        _logger.LogInformation("Status do pagamento: {Status}", pagamento.Status.ToString());

        await EnviarMensagemFila(pagamento, dadosPagamento);
    }

    private async Task<PagamentoEntity?> BuscarPagamentoDuplicadoOrderPlaced(OrderPlacedEvent dadosPagamento)
    {
        var pagamento = new PagamentoEntity(
            dadosPagamento.UserId,
            dadosPagamento.GameId,
            dadosPagamento.Price
            );

        return await _pagamentoRepository.ObterPorPedidoAsync(pagamento.PedidoId);
    }

    private async Task<PagamentoEntity> GerarPagamento(PagamentoRequest dadosPagamento)
    {
        var pagamento = await CriarPagamento(dadosPagamento);

        var pago = await ProcessarPagamento();

        await SalvarPagamento(pagamento, pago);

        return pagamento;
    }

    private async Task<PagamentoEntity> BuscarPagamento(Guid id)
    {
        var pagamento = await _pagamentoRepository.ObterPorIdAsync(id);

        if (pagamento is null)
            throw new KeyNotFoundException($"Pagamento {id} não encontrado");

        return pagamento;
    }

    private async Task ChecarPagamentoDuplicado(PagamentoRequest pagamentoRequest)
    {
        var pagamento = new PagamentoEntity(
            pagamentoRequest.UsuarioId,
            pagamentoRequest.JogoId,
            pagamentoRequest.Valor);

        var pagamentoExistente = await _pagamentoRepository.ObterPorPedidoAsync(pagamento.PedidoId);

        if (pagamentoExistente is not null && pagamentoExistente.Status == PagamentoStatus.Approved)
            throw new PagamentoJaPagoException();
    }

    private async Task<PagamentoEntity> CriarPagamento(PagamentoRequest dadosPagamento)
    {
        var pagamento = new PagamentoEntity(
            dadosPagamento.UsuarioId,
            dadosPagamento.JogoId,
            dadosPagamento.Valor
            );

        _pagamentoRepository.Adicionar(pagamento);
        await _pagamentoRepository.UnitOfWork.Commit();

        return pagamento;
    }

    private async Task<bool> ProcessarPagamento()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));

        return Random.Shared.NextDouble() >= _taxaReprovacao;
    }

    private async Task SalvarPagamento(PagamentoEntity pagamento, bool pago)
    {
        if (!pago)
            pagamento.RecusarPagamento();
        else
            pagamento.AprovarPagamento();

        _pagamentoRepository.Atualizar(pagamento);
        await _pagamentoRepository.UnitOfWork.Commit();
    }

    private async Task EnviarMensagemFila(PagamentoEntity pagamento, OrderPlacedEvent evento)
    {
        var message = new PaymentProcessedEvent(
            OrderId: pagamento.PedidoId,
            PaymentId: pagamento.Id,
            UserId: evento.UserId,
            GameId: evento.GameId,
            Price: evento.Price,
            Email: evento.Email,
            Status: pagamento.Status.ToString()
        );

        _logger.LogInformation("Enviando mensagem de PaymentProcessedEvent do pagamento: {PaymentId}", pagamento.Id);

        await _publishEndpoint.Publish(message);
    }
}
