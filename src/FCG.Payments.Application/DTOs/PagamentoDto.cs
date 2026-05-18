using Domain.Entities;
using Domain.Enums;

namespace Application.DTOs;

public class PagamentoDto
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid JogoId { get; set; }
    public decimal Valor { get; set; }
    public string? Status { get; set; }

    public static explicit operator PagamentoDto(PagamentoEntity entity)
    {
        return new PagamentoDto
        {
            Id = entity.Id,
            PedidoId = entity.PedidoId,
            UsuarioId = entity.UsuarioId,
            JogoId = entity.JogoId,
            Valor = entity.Valor,
            Status = entity.Status.ToString(),
        };
    }
}
