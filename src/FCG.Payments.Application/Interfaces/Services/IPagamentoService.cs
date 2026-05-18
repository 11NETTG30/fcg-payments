using Application.DTOs;
using Domain.Entities;
using FCG.Shared.Contracts.Events;

namespace Application.Interfaces.Services;

public interface IPagamentoService
{
    Task<PagamentoEntity> ProcessarAsync(PagamentoRequest dadosPagamento);
    Task ProcessarAsync(OrderPlacedEvent dadosPagamento);
    Task<PagamentoDto> ObterPagamentoAsync(Guid id);
    Task<List<PagamentoDto>> ListarPagamentosPorUsuarioAsync(Guid idUsuario);

}
