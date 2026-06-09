using FluentAssertions;
using TractorRental.Domain.Aggregates;
using TractorRental.Domain.Enums;
using TractorRental.Domain.Events;

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

        // A prova de que o Evento de Domínio foi gerado internamente
        contrato.DomainEvents.Should().ContainSingle();
        var domainEvent = contrato.DomainEvents.First() as ContratoIniciadoEvent;

        domainEvent.Should().NotBeNull();
        domainEvent!.TratorId.Should().Be(tratorId);
    }
}