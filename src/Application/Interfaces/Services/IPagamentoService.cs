using Application.DTOs;

namespace Application.Interfaces.Services;

public interface IPagamentoService
{
    Task<bool> ProcessarAsync(PagamentoDto dadosPagamento);
    Task<PagamentoDto> ObterPagamentoAsync(Guid id);
    Task<List<PagamentoDto>> ListarPagamentosPorUsuarioAsync(Guid idUsuario);

}
