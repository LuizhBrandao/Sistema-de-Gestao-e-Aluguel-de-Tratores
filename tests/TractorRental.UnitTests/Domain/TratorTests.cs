using FluentAssertions;
using TractorRental.Frota.Domain.Aggregates;
using TractorRental.Frota.Domain.Enums;
using TractorRental.Frota.Domain.Events;

namespace TractorRental.UnitTests.Domain;

public class TratorTests
{
    private static Trator CriarTratorValido(string modelo = "8R 370", MarcaTrator marca = MarcaTrator.JohnDeere)
        => new(Guid.NewGuid(), marca, modelo, 2022, 370, 1250.0, "1LV8370RCNR000123");

    // ── Testes de criação ──────────────────────────────────────────────

    [Fact]
    public void CriarTrator_ComDadosValidos_DeveCriarComSucesso()
    {
        // Act
        var trator = CriarTratorValido();

        // Assert
        trator.Marca.Should().Be(MarcaTrator.JohnDeere);
        trator.Modelo.Should().Be("8R 370");
        trator.AnoFabricacao.Should().Be(2022);
        trator.PotenciaCv.Should().Be(370);
        trator.HorimetroInicial.Should().Be(1250.0);
        trator.NumeroSerie.Should().Be("1LV8370RCNR000123");
        trator.Status.Should().Be(StatusTrator.Operacional);
    }

    [Fact]
    public void CriarTrator_ComModeloVazio_DeveLancarException()
    {
        // Act
        var act = () => new Trator(Guid.NewGuid(), MarcaTrator.Valtra, "", 2022, 250, 0, "PIN123");

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("modelo");
    }

    [Fact]
    public void CriarTrator_ComAnoInvalido_DeveLancarException()
    {
        // Act
        var act = () => new Trator(Guid.NewGuid(), MarcaTrator.NewHolland, "T7", 1900, 180, 0, "PIN123");

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("anoFabricacao");
    }

    [Fact]
    public void CriarTrator_ComPotenciaZero_DeveLancarException()
    {
        // Act
        var act = () => new Trator(Guid.NewGuid(), MarcaTrator.CaseIH, "Magnum", 2022, 0, 0, "PIN123");

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("potenciaCv");
    }

    [Fact]
    public void CriarTrator_ComHorimetroNegativo_DeveLancarException()
    {
        // Act
        var act = () => new Trator(Guid.NewGuid(), MarcaTrator.Agrale, "5075", 2020, 75, -10, "PIN123");

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("horimetroInicial");
    }

    [Fact]
    public void CriarTrator_ComNumeroSerieVazio_DeveLancarException()
    {
        // Act
        var act = () => new Trator(Guid.NewGuid(), MarcaTrator.Kubota, "M7", 2023, 170, 0, "");

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("numeroSerie");
    }

    // ── Testes existentes (atualizados) ────────────────────────────────

    [Fact]
    public void AtualizarSensores_DeveAtualizarMetricas()
    {
        // Arrange
        var trator = CriarTratorValido("8R 370", MarcaTrator.JohnDeere);

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
        var trator = CriarTratorValido("T250", MarcaTrator.Valtra);

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