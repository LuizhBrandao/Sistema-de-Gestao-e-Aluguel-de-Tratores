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
        var nome = "Empresa Agrícola SA";
        var docStr = "12.345.678/0001-99";
        
        var documento = new DocumentoIdentificacao(docStr, TipoPessoa.Juridica);
        var contato = new ContatoOperacional("João", "11999999999", "joao@empresa.com");
        var endereco = new Endereco("Rua A", "São Paulo", "SP");

        // Act
        var cliente = new Cliente(
            clienteId, 
            documento, 
            nome, 
            "ISENTO", 
            "financeiro@empresa.com", 
            contato, 
            endereco);

        // Assert
        cliente.Id.Should().Be(clienteId);
        cliente.RazaoSocialOuNome.Should().Be(nome);
        cliente.Documento.Numero.Should().Be("12345678000199");
        cliente.Documento.Tipo.Should().Be(TipoPessoa.Juridica);
        cliente.EmailFaturamento.Should().Be("financeiro@empresa.com");
        cliente.ContatoOperacional.Nome.Should().Be("João");
    }
}