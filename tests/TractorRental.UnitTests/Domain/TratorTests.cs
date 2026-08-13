using FluentAssertions;
using TractorRental.Frota.Domain.Aggregates;
using TractorRental.Frota.Domain.Enums;
using TractorRental.Frota.Domain.Events;

namespace TractorRental.UnitTests.Domain;

public class TratorTests
{
    [Fact]
    public void AtualizarSensores_DeveAtualizarMetricas()
    {
        // Arrange
        var trator = new Trator(Guid.NewGuid(), "John Deere 8R");

        // Act
        trator.AtualizarSensores(90.5, 32.0, 50.0, 85.0, 1500.0, 15.0);

        // Assert
        trator.TemperaturaAtualMotor.Should().Be(90.5);
        trator.PressaoAtualPneus.Should().Be(32.0);
        trator.NivelCombustivel.Should().Be(50.0);
        trator.NivelOleo.Should().Be(85.0);
        trator.RotacaoMotor.Should().Be(1500.0);
        trator.Velocidade.Should().Be(15.0);
    }

    [Fact]
    public void RegistrarAlertaManutencao_DeveAlterarStatus_EGerarEvento()
    {
        // Arrange
        var trator = new Trator(Guid.NewGuid(), "Valtra T250");

        // Act
        trator.RegistrarAlertaManutencao("Motor superaquecido");

        // Assert
        trator.Status.Should().Be(StatusTrator.EmManutencao);

        trator.DomainEvents.Should().ContainSingle();
        var domainEvent = trator.DomainEvents.First() as AlertaGeradoEvent;

        domainEvent.Should().NotBeNull();
        domainEvent!.Motivo.Should().Be("Motor superaquecido");
        domainEvent.Criticidade.Should().Be("ALTA");
    }
}