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

    public async Task SaveAsync(Guid userId, Guid gameId)
    {
        _context.ProcessedEvents.Add(new ProcessedEventEntity
        {
            UserId = userId,
            GameId = gameId,
            ProcessedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }
}