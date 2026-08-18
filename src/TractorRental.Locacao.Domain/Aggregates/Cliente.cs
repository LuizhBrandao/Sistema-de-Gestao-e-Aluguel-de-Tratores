using TractorRental.Locacao.Domain.Enums;

namespace TractorRental.Locacao.Domain.Aggregates;

public class Cliente
{
    public Guid Id { get; private set; }
    
    // Dados Fiscais
    public string DocumentoNumero { get; private set; }
    public TipoPessoa DocumentoTipo { get; private set; }
    public string RazaoSocialOuNome { get; private set; }
    public string? InscricaoEstadual { get; private set; }
    
    // Contato
    public string EmailFaturamento { get; private set; }
    public string ContatoNome { get; private set; }
    public string ContatoTelefone { get; private set; }
    public string ContatoEmail { get; private set; }

    // Endereço de Entrega/Operação
    public string EnderecoLogradouro { get; private set; }
    public string EnderecoCidade { get; private set; }
    public string EnderecoEstado { get; private set; }

    protected Cliente() { }

    public Cliente(
        Guid id, 
        string documentoNumero,
        TipoPessoa documentoTipo,
        string razaoSocialOuNome, 
        string? inscricaoEstadual, 
        string emailFaturamento, 
        string contatoNome,
        string contatoTelefone,
        string contatoEmail,
        string enderecoLogradouro,
        string enderecoCidade,
        string enderecoEstado)
    {
        if (string.IsNullOrWhiteSpace(razaoSocialOuNome)) throw new ArgumentException("Razão Social/Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(emailFaturamento)) throw new ArgumentException("E-mail de faturamento é obrigatório.");

        Id = id;
        DocumentoNumero = documentoNumero;
        DocumentoTipo = documentoTipo;
        RazaoSocialOuNome = razaoSocialOuNome;
        InscricaoEstadual = inscricaoEstadual;
        EmailFaturamento = emailFaturamento;
        ContatoNome = contatoNome;
        ContatoTelefone = contatoTelefone;
        ContatoEmail = contatoEmail;
        EnderecoLogradouro = enderecoLogradouro;
        EnderecoCidade = enderecoCidade;
        EnderecoEstado = enderecoEstado;
    }
}
