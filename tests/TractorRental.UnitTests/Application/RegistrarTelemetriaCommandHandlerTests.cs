using FluentAssertions;
using MediatR;
using Moq;
using TractorRental.Telemetria.Application.Commands;
using TractorRental.SharedKernel.Events;

namespace TractorRental.UnitTests.Application;

public class RegistrarTelemetriaCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly RegistrarTelemetriaCommandHandler _handler;

    public RegistrarTelemetriaCommandHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new RegistrarTelemetriaCommandHandler(_mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_DevePublicarEventoETornarSucesso()
    {
        // Arrange
        var command = new RegistrarTelemetriaCommand(Guid.NewGuid(), 95.0, 30.0, 80.0, 85.0, 1500.0, 20.0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        // Deve ter publicado TelemetriaProcessadaIntegrationEvent
        _mediatorMock.Verify(m => m.Publish(
            It.Is<object>(e => e is TelemetriaProcessadaIntegrationEvent),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}