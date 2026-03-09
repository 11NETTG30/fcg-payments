namespace Domain.Exceptions;

public class PedidoCanceladoException : DomainException
{
    public PedidoCanceladoException()
        : base("O pedido está cancelado.")
    {
    }
}
