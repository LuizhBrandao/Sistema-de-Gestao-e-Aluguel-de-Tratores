using FluentAssertions;
using TractorRental.Locacao.Domain.Aggregates;
using TractorRental.Locacao.Domain.Enums;

namespace TractorRental.UnitTests.Domain;

public class ClienteTests
{
    [Fact]
    public void InstanciarCliente_DeveCriarComDadosCorretos()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var nome = "João Silva";
        
        var cliente = new Cliente(
            clienteId, 
            "12345678901",
            TipoPessoa.Fisica,
            nome, 
            null, 
            "joao@email.com", 
            "João",
            "11999999999",
            "joao@email.com",
            "Rua A",
            "São Paulo",
            "SP");

        // Assert
        cliente.Id.Should().Be(clienteId);
        cliente.RazaoSocialOuNome.Should().Be(nome);
        Assert.Equal("12345678901", cliente.DocumentoNumero);
        Assert.Equal(TipoPessoa.Fisica, cliente.DocumentoTipo);
        Assert.Equal("João Silva", cliente.RazaoSocialOuNome);
        Assert.Equal("João", cliente.ContatoNome);
        Assert.Equal("11999999999", cliente.ContatoTelefone);
    }
}