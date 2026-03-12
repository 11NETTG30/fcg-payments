using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Tests.Domain.Tests.Entities;

public class PagamentoEntityTests
{
    private PagamentoEntity CriarPagamentoValido()
    {
        return new PagamentoEntity(
                Guid.NewGuid(),
                Guid.NewGuid(),
                100);
    }

    [Fact]
    public void Deve_criar_pagamento_com_status_criado()
    {
        var pagamento = CriarPagamentoValido();

        Assert.Equal(PagamentoStatus.Criado, pagamento.Status);
        Assert.NotEqual(Guid.Empty, pagamento.Id);
    }

    [Fact]
    public void Deve_recusar_pagamento_com_status_criado()
    {
        var pagamento = CriarPagamentoValido();

        pagamento.RecusarPagamento();

        Assert.Equal(PagamentoStatus.PagamentoRecusado, pagamento.Status);
    }

    [Fact]
    public void Nao_deve_pagar_com_status_cancelado()
    {
        var pagamento = CriarPagamentoValido();

        pagamento.Cancelar();

        var exceptionAprovar = Assert.Throws<PagamentoCanceladoException>(() =>
            pagamento.AprovarPagamento());

        var exceptionRecusar = Assert.Throws<DomainException>(() =>
            pagamento.RecusarPagamento());

        Assert.Equal("O pagamento está cancelado.", exceptionAprovar.Message);
        Assert.Equal("Pagamento não pode ser recusado neste estado.", exceptionRecusar.Message);
    }

    [Fact]
    public void Nao_deve_aprovar_pagamento_duas_vezes()
    {
        var pagamento = CriarPagamentoValido();

        pagamento.AprovarPagamento();

        var exception = Assert.Throws<PagamentoJaPagoException>(() =>
            pagamento.AprovarPagamento());

        Assert.Equal("O pagamento já foi feito anteriormente.", exception.Message);
    }

    [Fact]
    public void Nao_deve_criar_pagamento_com_valor_zero()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new PagamentoEntity(
                Guid.NewGuid(),
                Guid.NewGuid(),
                0)
            );

        Assert.Equal("Valor do pagamento deve ser maior que zero", exception.Message);
    }

    [Fact]
    public void Nao_deve_criar_pagamento_com_valor_negativo()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new PagamentoEntity(
                Guid.NewGuid(),
                Guid.NewGuid(),
                -100)
            );

        Assert.Equal("Valor do pagamento deve ser maior que zero", exception.Message);
    }
}
