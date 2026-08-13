using MediatR;
using Moq;
using TractorRental.SharedKernel.Events;
using TractorRental.Telemetria.Application.Policies;

namespace TractorRental.UnitTests.Application;

public class RiscoManutencaoPolicyTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly RiscoManutencaoPolicy _policy;

    public RiscoManutencaoPolicyTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _policy = new RiscoManutencaoPolicy(_mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_TemperaturaAcimaDoLimite_DevePublicarAlerta()
    {
        // Arrange — Temperatura = 115.0 (Crítico > 110)
        var evento = new TelemetriaProcessadaIntegrationEvent(
            Guid.NewGuid(), 115.0, 30.0, 80.0, 80.0, 2000.0, 10.0, DateTime.UtcNow);

        // Act
        await _policy.Handle(evento, CancellationToken.None);

        // Assert — Deve ter publicado AnomaliaDetectadaIntegrationEvent
        _mediatorMock.Verify(m => m.Publish(
            It.Is<object>(e => e is AnomaliaDetectadaIntegrationEvent),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NivelOleoCritico_DevePublicarAlerta()
    {
        // Arrange — Óleo = 10.0 (Crítico < 15)
        var evento = new TelemetriaProcessadaIntegrationEvent(
            Guid.NewGuid(), 90.0, 30.0, 80.0, 10.0, 2000.0, 10.0, DateTime.UtcNow);

        // Act
        await _policy.Handle(evento, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Publish(
            It.Is<object>(e => e is AnomaliaDetectadaIntegrationEvent),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CenarioNormal_NaoDevePublicarAlerta()
    {
        // Arrange — Todos os valores normais
        var evento = new TelemetriaProcessadaIntegrationEvent(
            Guid.NewGuid(), 90.0, 30.0, 80.0, 85.0, 1500.0, 20.0, DateTime.UtcNow);

        // Act
        await _policy.Handle(evento, CancellationToken.None);

        // Assert — Não deve ter publicado nenhum alerta
        _mediatorMock.Verify(m => m.Publish(
            It.IsAny<object>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}