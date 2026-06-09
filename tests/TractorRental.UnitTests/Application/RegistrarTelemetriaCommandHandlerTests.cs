using FluentAssertions;
using FluentAssertions.Equivalency.Tracing;
using MediatR;
using Moq;
using TractorRental.Application.Commands;
using TractorRental.Application.Interfaces;
using TractorRental.Domain.Aggregates;

namespace TractorRental.UnitTests.Application;

public class RegistrarTelemetriaCommandHandlerTests
{
    private readonly Mock<ITratorRepository> _repositoryMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly RegistrarTelemetriaCommandHandler _handler;

    public RegistrarTelemetriaCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITratorRepository>();
        _mediatorMock = new Mock<IMediator>();
        _handler = new RegistrarTelemetriaCommandHandler(_repositoryMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_QuandoTratorExiste_DeveProcessarESalvar()
    {
        // Arrange
        var tratorId = Guid.NewGuid();
        var trator = new Trator(tratorId, "Massey Ferguson");

        _repositoryMock.Setup(r => r.ObterPorIdAsync(tratorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trator);

        var command = new RegistrarTelemetriaCommand(tratorId, 95.0, 30.0, 80.0, 85.0, 1500.0, 20.0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        trator.TemperaturaAtualMotor.Should().Be(95.0);

        _repositoryMock.Verify(r => r.AtualizarAsync(trator, It.IsAny<CancellationToken>()), Times.Once);
        // Apague a linha que está dando erro e coloque esta:
        _mediatorMock.Verify(m => m.Publish(It.Is<object>(e => e is TractorRental.Domain.Events.LeituraRecebidaEvent), It.IsAny<CancellationToken>()), Times.Once);
        trator.DomainEvents.Should().BeEmpty(); // Validando se o handler chamou o LimparEventos()
    }

    [Fact]
    public async Task Handle_QuandoTratorNaoExiste_DeveRetornarFalso()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Trator?)null);

        var command = new RegistrarTelemetriaCommand(Guid.NewGuid(), 95.0, 30.0, 80.0, 85.0, 1500.0, 20.0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Trator>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}