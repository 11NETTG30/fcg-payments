using Application.Configuration;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using FCG.Shared.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests.Domain.Tests.Application;

public class PagamentoServiceTests
{
    private readonly Mock<IPagamentoRepository> _pagamentoRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPublishEndpoint> _eventPublisherMock;
    private readonly Mock<ILogger<PagamentoService>> _loggerMock;
    private readonly Mock<IOptions<PagamentoOptions>> _pagamentoOptionsMock;

    public PagamentoServiceTests()
    {
        _pagamentoRepoMock = new Mock<IPagamentoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _eventPublisherMock = new Mock<IPublishEndpoint>();
        _loggerMock = new Mock<ILogger<PagamentoService>>();
        _pagamentoOptionsMock = new Mock<IOptions<PagamentoOptions>>();

        _pagamentoRepoMock
            .Setup(r => r.UnitOfWork)
            .Returns(_unitOfWorkMock.Object);

        _unitOfWorkMock
            .Setup(u => u.Commit())
            .ReturnsAsync(true);
    }

    private static PagamentoRequest CriarPagamentoRequest()
    {
        return new PagamentoRequest
        {
            JogoId = Guid.NewGuid(),
            UsuarioId = Guid.NewGuid(),
            Valor = 100
        };
    }

    private static OrderPlacedEvent CriarPagamentoEvent()
    {
        return new OrderPlacedEvent(
            GameId: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            Price: 100,
            Email: "usuario@teste.com"
        );
    }

    private PagamentoEntity CriarPagamentoValido()
    {
        return new PagamentoEntity(
                Guid.NewGuid(),
                Guid.NewGuid(),
                100);
    }

    private PagamentoService CriarService(double taxaReprovacao)
    {
        _pagamentoOptionsMock
            .Setup(o => o.Value)
            .Returns(new PagamentoOptions { TaxaReprovacao = taxaReprovacao });

        return new PagamentoService(
            _pagamentoRepoMock.Object,
            _eventPublisherMock.Object,
            _loggerMock.Object,
            _pagamentoOptionsMock.Object
        );
    }

    [Fact]
    public async Task Deve_criar_pagamento_e_salvar_no_repositorio_quando_aprovado()
    {
        var service = CriarService(taxaReprovacao: 0.0);
        var pagamento = CriarPagamentoRequest();

        await service.ProcessarAsync(pagamento);

        _pagamentoRepoMock.Verify(r => r.Adicionar(It.IsAny<PagamentoEntity>()), Times.AtLeastOnce);
        _pagamentoRepoMock.Verify(r => r.UnitOfWork.Commit(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Deve_lancar_exception_quando_pagamento_reprovado()
    {
        var service = CriarService(taxaReprovacao: 1);
        var pagamento = CriarPagamentoRequest();

        await Assert.ThrowsAsync<PagamentoRecusadoException>(
            () => service.ProcessarAsync(pagamento)
        );
    }

    [Fact]
    public async Task Deve_criar_pagamento_e_publicar_mensagem()
    {
        // Arrange
        var service = CriarService(taxaReprovacao: 0);
        var pagamento = CriarPagamentoEvent();

        // Act
        await service.ProcessarAsync(pagamento);

        // Assert
        _pagamentoRepoMock.Verify(r => r.Adicionar(It.IsAny<PagamentoEntity>()), Times.AtLeastOnce);
        _pagamentoRepoMock.Verify(r => r.UnitOfWork.Commit(), Times.AtLeastOnce);

        _eventPublisherMock.Verify(p => p.Publish(
                It.Is<PaymentProcessedEvent>(msg =>
                    msg.GameId == pagamento.GameId &&
                    msg.UserId == pagamento.UserId &&
                    msg.Email == pagamento.Email &&
                    msg.Price == pagamento.Price &&
                    msg.PaymentId != Guid.Empty)
                ),
                Times.Once);
    }

    [Fact]
    public async Task Deve_buscar_pagamento_por_id()
    {
        // Arrange
        var service = CriarService(taxaReprovacao: 0.0);
        var pagamento = CriarPagamentoValido();

        _pagamentoRepoMock
            .Setup(r => r.ObterPorIdAsync(pagamento.Id))
            .ReturnsAsync(pagamento);

        // Act
        var resultado = await service.ObterPagamentoAsync(pagamento.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(pagamento.Id, resultado.Id);

        _pagamentoRepoMock.Verify(r => r.ObterPorIdAsync(pagamento.Id), Times.Once);
        _pagamentoRepoMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
    }


}
