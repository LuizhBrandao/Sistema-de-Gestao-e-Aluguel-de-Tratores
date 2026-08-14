using TractorRental.Locacao.Domain.Enums;

namespace TractorRental.Locacao.Domain.Aggregates;

public class Cliente
{
    public Guid Id { get; private set; }
    
    // Dados Fiscais
    public DocumentoIdentificacao Documento { get; private set; }
    public string RazaoSocialOuNome { get; private set; }
    public string? InscricaoEstadual { get; private set; }
    
    // Contato
    public string EmailFaturamento { get; private set; }
    public ContatoOperacional ContatoOperacional { get; private set; }

    // Endereço de Entrega/Operação
    public Endereco EnderecoOperacao { get; private set; }

    protected Cliente() { }

    public Cliente(
        Guid id, 
        DocumentoIdentificacao documento, 
        string razaoSocialOuNome, 
        string? inscricaoEstadual, 
        string emailFaturamento, 
        ContatoOperacional contatoOperacional, 
        Endereco enderecoOperacao)
    {
        if (string.IsNullOrWhiteSpace(razaoSocialOuNome)) throw new ArgumentException("Razão Social/Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(emailFaturamento)) throw new ArgumentException("E-mail de faturamento é obrigatório.");

        Id = id;
        Documento = documento ?? throw new ArgumentNullException(nameof(documento));
        RazaoSocialOuNome = razaoSocialOuNome;
        InscricaoEstadual = inscricaoEstadual;
        EmailFaturamento = emailFaturamento;
        ContatoOperacional = contatoOperacional ?? throw new ArgumentNullException(nameof(contatoOperacional));
        EnderecoOperacao = enderecoOperacao ?? throw new ArgumentNullException(nameof(enderecoOperacao));
    }
}
