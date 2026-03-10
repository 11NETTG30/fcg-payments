using Application.DTOs;
using Application.Events;
using Application.Interfaces;
using Application.Interfaces.Interfaces.Repositories;
using Application.Interfaces.Messaging;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Moq;

namespace Tests.Domain.Tests.Application;

public class PagamentoServiceTests
{
    private readonly Mock<IPagamentoRepository> _pagamentoRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly PagamentoService _service;

    public PagamentoServiceTests()
    {
        _pagamentoRepoMock = new Mock<IPagamentoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _eventPublisherMock = new Mock<IEventPublisher>();

        _pagamentoRepoMock
            .Setup(r => r.UnitOfWork)
            .Returns(_unitOfWorkMock.Object);

        _unitOfWorkMock
            .Setup(u => u.Commit())
            .ReturnsAsync(true);

        _service = new PagamentoService(
            _pagamentoRepoMock.Object,
            _eventPublisherMock.Object
        );

    }

    private static PagamentoRequest CriarPagamentoRequest()
    {
        return new PagamentoRequest
        {
            JogoId = Guid.NewGuid(),
            PedidoId = Guid.NewGuid(),
            UsuarioId = Guid.NewGuid(),
            Valor = 100
        };
    }

    private static OrderPlacedEvent CriarPagamentoEvent()
    {
        return new OrderPlacedEvent
        {
            OrderId = Guid.NewGuid(),
            GameId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Price = 100
        };
    }

    private PagamentoEntity CriarPagamentoValido()
    {
        return new PagamentoEntity(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                100);
    }

    [Fact]
    public async Task Deve_criar_pagamento_e_salvar_no_repositorio()
    {
        var pagamento = CriarPagamentoRequest();

        await _service.ProcessarAsync(pagamento);

        _pagamentoRepoMock.Verify(r => r.Adicionar(It.IsAny<PagamentoEntity>()), Times.AtLeastOnce);
        _pagamentoRepoMock.Verify(r => r.UnitOfWork.Commit(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Deve_criar_pagamento_e_publicar_mensagem()
    {
        // Arrange
        var pagamento = CriarPagamentoEvent();

        // Act
        await _service.ProcessarAsync(pagamento);

        // Assert
        _pagamentoRepoMock.Verify(r => r.Adicionar(It.IsAny<PagamentoEntity>()), Times.AtLeastOnce);
        _pagamentoRepoMock.Verify(r => r.UnitOfWork.Commit(), Times.AtLeastOnce);

        _eventPublisherMock.Verify(p => p.PublishAsync(
                It.Is<PaymentProcessedEvent>(msg =>
                    msg.PaymentStatus == PagamentoStatus.Pago.ToString() &&
                    msg.PaymentId != Guid.Empty),
                "payment-processed"),
                Times.Once);
    }

    [Fact]
    public async Task Deve_buscar_pagamento_por_id()
    {
        // Arrange
        var pagamento = CriarPagamentoValido();

        _pagamentoRepoMock
            .Setup(r => r.ObterPorIdAsync(pagamento.Id))
            .ReturnsAsync(pagamento);

        // Act
        var resultado = await _service.ObterPagamentoAsync(pagamento.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(pagamento.Id, resultado.Id);

        _pagamentoRepoMock.Verify(r => r.ObterPorIdAsync(pagamento.Id), Times.Once);
        _pagamentoRepoMock.Verify(r => r.UnitOfWork.Commit(), Times.Never);
    }


}
