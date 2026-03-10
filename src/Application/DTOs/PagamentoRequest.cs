namespace Application.DTOs;

public class PagamentoRequest
{
    public Guid PedidoId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid JogoId { get; set; }
    public decimal Valor { get; set; }

    public static explicit operator PagamentoRequest(OrderPlacedEvent evento)
    {
        return new PagamentoRequest
        {
            PedidoId = evento.OrderId,
            UsuarioId = evento.UserId,
            JogoId = evento.GameId,
            Valor = evento.Price
        };
    }
}
