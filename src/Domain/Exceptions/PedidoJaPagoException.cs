namespace Domain.Exceptions;

public class PedidoJaPagoException : DomainException
{
    public PedidoJaPagoException()
        : base("O pedido já foi pago anteriormente.")
    {
    }
}
