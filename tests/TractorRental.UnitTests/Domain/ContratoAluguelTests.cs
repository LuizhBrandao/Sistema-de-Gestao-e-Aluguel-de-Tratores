using FluentAssertions;
using TractorRental.Locacao.Domain.Aggregates;
using TractorRental.Locacao.Domain.Enums;
using TractorRental.SharedKernel.Events;

namespace TractorRental.UnitTests.Domain;

public class ContratoAluguelTests
{
    [Fact]
    public void InstanciarContrato_DeveGerarEventoDeDominio_E_EstarAtivo()
    {
        // Arrange
        var contratoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var tratorId = Guid.NewGuid();
        var valorHora = 150.00m;

        // Act
        var contrato = new ContratoAluguel(contratoId, clienteId, tratorId, valorHora);

        // Assert
        contrato.Id.Should().Be(contratoId);
        contrato.Status.Should().Be(StatusContrato.Ativo);

        // A prova de que o Integration Event foi gerado internamente
        contrato.DomainEvents.Should().ContainSingle();
        var domainEvent = contrato.DomainEvents.First() as ContratoIniciadoIntegrationEvent;

        domainEvent.Should().NotBeNull();
        domainEvent!.TratorId.Should().Be(tratorId);
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