using Application.DTOs;
using FluentValidation;

namespace Application.Validators;

public sealed class PagamentoRequestValidator : AbstractValidator<PagamentoRequest>
{
    public PagamentoRequestValidator()
    {
        RuleFor(request => request.PedidoId)
            .NotEmpty();

        RuleFor(request => request.JogoId)
            .NotEmpty();

        RuleFor(request => request.UsuarioId)
            .NotEmpty();

        RuleFor(request => request.Valor)
            .GreaterThan(0)
            .WithMessage("O valor deve ser maior que 0.");
    }
}
