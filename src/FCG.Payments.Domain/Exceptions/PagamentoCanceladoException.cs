namespace Domain.Exceptions;

public class PagamentoCanceladoException : DomainException
{
    public PagamentoCanceladoException()
        : base("O pagamento está cancelado.")
    {
    }
}
