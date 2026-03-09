using Application.DTOs;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Repositories;

namespace Application.Services;

public class PagamentoService : IPagamentoService
{
    private readonly IPagamentoRepository _pagamentoRepository;

    public PagamentoService(IPagamentoRepository pagamentoRepository)
    {
        _pagamentoRepository = pagamentoRepository;
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
        await ChecarPagamentoDuplicado(dadosPagamento.PedidoId);

        var pagamento = await CriarPedido(dadosPagamento);

        var pago = ProcessarPagamento();

        await SalvarPagamento(pagamento, pago);

        if (pagamento.Status != PagamentoStatus.Pago)
            throw new PagamentoRecusadoException(pagamento.Id);

        return pagamento;
    }

    private async Task<PagamentoEntity> BuscarPagamento(Guid id)
    {
        var pagamento = await _pagamentoRepository.ObterPorIdAsync(id);

        if (pagamento is null)
            throw new KeyNotFoundException($"Pagamento {id} não encontrado");

        return pagamento;
    }

    private async Task ChecarPagamentoDuplicado(Guid pedidoId)
    {
        var pagamentoExistente = await _pagamentoRepository.ObterPorPedidoAsync(pedidoId);

        if (pagamentoExistente is not null && pagamentoExistente.Status == PagamentoStatus.Pago)
            throw new PedidoJaPagoException();
    }

    private async Task<PagamentoEntity> CriarPedido(PagamentoRequest dadosPagamento)
    {
        var pagamento = new PagamentoEntity(
            dadosPagamento.PedidoId,
            dadosPagamento.UsuarioId,
            dadosPagamento.JogoId,
            dadosPagamento.Valor
            );

        _pagamentoRepository.Adicionar(pagamento);
        await _pagamentoRepository.UnitOfWork.Commit();

        return pagamento;
    }

    private bool ProcessarPagamento()
    {
        return true;
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
}
