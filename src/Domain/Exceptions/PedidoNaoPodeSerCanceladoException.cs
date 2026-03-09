namespace Domain.Exceptions;

public class PedidoNaoPodeSerCanceladoException : DomainException
{
    public PedidoNaoPodeSerCanceladoException()
        : base("O pedido não pode ser cancelado no estado atual.")
    {
    }
}
