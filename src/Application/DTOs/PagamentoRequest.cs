namespace Application.DTOs;

public class PagamentoRequest
{
    public Guid PedidoId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid JogoId { get; set; }
    public decimal Valor { get; set; }
}
