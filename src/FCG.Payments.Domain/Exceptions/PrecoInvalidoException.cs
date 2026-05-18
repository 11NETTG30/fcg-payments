namespace Domain.Exceptions;

public class PrecoInvalidoException : DomainException
{
    public PrecoInvalidoException()
        : base("O preço do item não pode ser negativo.") 
    {
    }
}
