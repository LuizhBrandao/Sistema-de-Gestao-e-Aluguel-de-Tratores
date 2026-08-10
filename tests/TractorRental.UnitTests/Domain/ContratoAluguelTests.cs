using FluentAssertions;
using TractorRental.Domain.Aggregates;
using TractorRental.Domain.Enums;
using TractorRental.Domain.Events;

namespace TractorRental.UnitTests.Domain;

public class ContratoAluguelTests
{
    [Fact]
    public void CriarContrato_DeveInicializarCorretamente_EGerarEvento()
    {
        // Arrange & Act
        var contrato = new ContratoAluguel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 150.00m);

        // Assert
        contrato.Status.Should().Be(StatusContrato.Ativo);
        contrato.DataFim.Should().BeNull();

        contrato.DomainEvents.Should().ContainSingle();
        contrato.DomainEvents.First().Should().BeOfType<ContratoIniciadoEvent>();
    }

    [Fact]
    public void FinalizarContrato_DeveAlterarStatus_EDefinirDataFim()
    {
        // Arrange
        var contrato = new ContratoAluguel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 150.00m);

        // Act
        contrato.FinalizarContrato();

        // Assert
        contrato.Status.Should().Be(StatusContrato.Finalizado);
        contrato.DataFim.Should().NotBeNull();
    }

    [Fact]
    public void CalcularFaturamento_DeveRetornarValorProporcionalAsHoras()
    {
        // Arrange
        var contrato = new ContratoAluguel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100.00m);
        var dataTeste = contrato.DataInicio.AddHours(2.5); // 2 horas e meia de uso

        // Act
        var faturamento = contrato.CalcularFaturamento(dataTeste);

        // Assert
        faturamento.Should().Be(250.00m);
    }
}