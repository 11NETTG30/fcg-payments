using Application.Interfaces;
using Domain.Entities;

namespace Domain.Repositories;

public interface IPagamentoRepository : IRepository<PagamentoEntity>
{
    Task<PagamentoEntity?> ObterPorIdAsync(Guid id);
    Task<PagamentoEntity?> ObterPorPedidoAsync(Guid idPedido);
    void Adicionar(PagamentoEntity pagamento);
    void Atualizar(PagamentoEntity pagamento);
    Task<List<PagamentoEntity>> ListarPagamentosPorUsuarioAsync(Guid idUsuario);
}
