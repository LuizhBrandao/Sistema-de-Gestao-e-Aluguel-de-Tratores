using FluentAssertions;
using TractorRental.Domain.Aggregates;

namespace TractorRental.UnitTests.Domain;

public class ClienteTests
{
    [Fact]
    public void InstanciarCliente_DeveCriarComDadosCorretos()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var nome = "Empresa Agrícola SA";
        var documento = "12.345.678/0001-99";

        // Act
        var cliente = new Cliente(clienteId, nome, documento);

        // Assert
        cliente.Id.Should().Be(clienteId);
        cliente.Nome.Should().Be(nome);
        cliente.Documento.Should().Be(documento);
    }
}