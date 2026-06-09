using MediatR;
using Moq;
using TractorRental.Application.Interfaces;
using TractorRental.Application.Policies;
using TractorRental.Domain.Aggregates;
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
    public async Task Handle_TemperaturaAcimaDoLimite_DeveBloquearTrator()
    {
        // Arrange
        var tratorId = Guid.NewGuid();
        var trator = new Trator(tratorId, "New Holland");
        _repositoryMock.Setup(r => r.ObterPorIdAsync(tratorId, It.IsAny<CancellationToken>())).ReturnsAsync(trator);

        // Temperatura = 115.0 (Crítico)
        var evento = new LeituraRecebidaEvent(tratorId, 115.0, 30.0, 80.0, 80.0, 2000.0, 10.0, DateTime.UtcNow);

        // Act
        await _policy.Handle(evento, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Trator>(), It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.Is<object>(e => e is AlertaGeradoEvent), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NivelOleoCritico_DeveBloquearTrator()
    {
        // Arrange
        var tratorId = Guid.NewGuid();
        var trator = new Trator(tratorId, "John Deere");
        _repositoryMock.Setup(r => r.ObterPorIdAsync(tratorId, It.IsAny<CancellationToken>())).ReturnsAsync(trator);

        // Óleo = 10.0 (Abaixo de 15 é Crítico)
        var evento = new LeituraRecebidaEvent(tratorId, 90.0, 30.0, 80.0, 10.0, 2000.0, 10.0, DateTime.UtcNow);

        // Act
        await _policy.Handle(evento, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Trator>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CenarioNormal_NaoDeveFazerNada()
    {
        // Arrange
        var evento = new LeituraRecebidaEvent(Guid.NewGuid(), 90.0, 30.0, 80.0, 85.0, 1500.0, 20.0, DateTime.UtcNow);

        // Act
        await _policy.Handle(evento, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Trator>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}