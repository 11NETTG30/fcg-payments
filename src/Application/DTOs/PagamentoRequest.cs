using Application.Events;

namespace Application.DTOs;

public class PagamentoRequest
{
    public Guid UsuarioId { get; set; }
    public Guid JogoId { get; set; }
    public decimal Valor { get; set; }

    public static explicit operator PagamentoRequest(OrderPlacedEvent evento)
    {
        return new PagamentoRequest
        {
            UsuarioId = evento.UserId,
            JogoId = evento.GameId,
            Valor = evento.Price
        };
    }
}
