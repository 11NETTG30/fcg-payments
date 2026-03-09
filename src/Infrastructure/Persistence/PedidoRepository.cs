using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class PedidoRepository : IPagamentoRepository
{
    private readonly PagamentoDbContext _context;

    public PedidoRepository(PagamentoDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public void Dispose()
    {
        _context?.Dispose();
    }


    public async Task<PagamentoEntity?> ObterPorIdAsync(Guid id)
    {
        return await _context.Pagamentos
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public void Adicionar(PagamentoEntity pedido)
    {
        _context.Pagamentos.Add(pedido);
    }

    public void Atualizar(PagamentoEntity pedido)
    {
        _context.Pagamentos.Update(pedido);
    }

    public async Task<List<PagamentoEntity>> ListarPagamentosPorUsuarioAsync(Guid idUsuario)
    {
        var dados = await _context.Pagamentos
            .Where(p => p.UsuarioId == idUsuario)
            .ToListAsync();

        return dados;
    }

    public async Task<PagamentoEntity?> ObterPorPedidoAsync(Guid idPedido)
    {
        var dado = await _context.Pagamentos
            .Where(p => p.PedidoId == idPedido)
            .FirstOrDefaultAsync();

        return dado;
    }
}
