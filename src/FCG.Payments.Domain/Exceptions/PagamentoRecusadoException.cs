namespace Domain.Exceptions;

public class PagamentoRecusadoException : DomainException
{
    public Guid PagamentoId { get; }

    public PagamentoRecusadoException(Guid id)
        : base("O pagamento foi recusado.") 
    {
        PagamentoId = id;
    }
}
