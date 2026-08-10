using FluentAssertions.Equivalency.Tracing;
using MediatR;
using Moq;
using TractorRental.Application.Interfaces;
using TractorRental.Application.Policies;
using TractorRental.Domain.Aggregates;
using TractorRental.Domain.Enums;
using TractorRental.Domain.Events;

namespace TractorRental.UnitTests.Application;

public class RiscoManutencaoPolicyTests
{
    private readonly Mock<ITratorRepository> _repositoryMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly RiscoManutencaoPolicy _policy;

    public RiscoManutencaoPolicyTests()
    {
        _repositoryMock = new Mock<ITratorRepository>();
        _mediatorMock = new Mock<IMediator>();
        _policy = new RiscoManutencaoPolicy(_repositoryMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_TemperaturaAcimaDoLimite_DeveGerarAlertaESalvar()
    {
        // Arrange
        var tratorId = Guid.NewGuid();
        var trator = new Trator(tratorId, "New Holland");

        _repositoryMock.Setup(r => r.ObterPorIdAsync(tratorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trator);

        // Temperatura acima de 110.0 (regra crítica)
        var eventoLeitura = new LeituraRecebidaEvent(tratorId, 115.0, 30.0, 80.0, DateTime.UtcNow);

        // Act
        await _policy.Handle(eventoLeitura, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.AtualizarAsync(trator, It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TemperaturaNormal_NaoDeveFazerNada()
    {
        // Arrange
        var eventoLeitura = new LeituraRecebidaEvent(Guid.NewGuid(), 90.0, 30.0, 80.0, DateTime.UtcNow);

        // Act
        await _policy.Handle(eventoLeitura, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}