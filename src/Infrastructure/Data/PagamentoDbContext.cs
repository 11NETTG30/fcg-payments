using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class PagamentoDbContext : DbContext, IUnitOfWork
{
    public PagamentoDbContext(DbContextOptions<PagamentoDbContext> options)
        : base(options)
    {
    }

    public DbSet<PagamentoEntity> Pagamentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PagamentoDbContext).Assembly);

        modelBuilder.HasDefaultSchema("pagamento");

        base.OnModelCreating(modelBuilder);
    }

    public async Task<bool> Commit()
    {
        return await SaveChangesAsync() > 0;
    }
}