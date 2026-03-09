namespace Domain.Exceptions;

public class PagamentoJaPagoException : DomainException
{
    public PagamentoJaPagoException()
        : base("O pagamento já foi feito anteriormente.")
    {
    }
}
