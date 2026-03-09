namespace Domain.Exceptions;

public class PagamentoNaoPodeSerCanceladoException : DomainException
{
    public PagamentoNaoPodeSerCanceladoException()
        : base("O pagamento não pode ser cancelado no estado atual.")
    {
    }
}
