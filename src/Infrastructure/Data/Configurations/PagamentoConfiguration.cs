using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<PagamentoEntity>
{
    public void Configure(EntityTypeBuilder<PagamentoEntity> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PedidoId)
            .IsRequired();

        builder.Property(p => p.UsuarioId)
            .IsRequired();

        builder.Property(p => p.JogoId)
            .IsRequired();

        builder.Property(p => p.DataCriacao)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.Valor)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.ToTable("Pagamentos");
    }
}