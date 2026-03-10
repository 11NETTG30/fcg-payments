using Application.Interfaces.Interfaces.Repositories;
using Infrastructure.Data;
using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProcessedEventRepository : IProcessedEventRepository
{
    private readonly PagamentoDbContext _context;

    public ProcessedEventRepository(PagamentoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid orderId)
    {
        return await _context.ProcessedEvents
            .AnyAsync(x => x.OrderId == orderId);
    }

    public async Task SaveAsync(Guid orderId)
    {
        _context.ProcessedEvents.Add(new ProcessedEventEntity
        {
            OrderId = orderId,
            ProcessedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }
}