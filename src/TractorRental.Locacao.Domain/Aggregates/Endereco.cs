namespace TractorRental.Locacao.Domain.Aggregates;

public class Endereco
{
    public string Logradouro { get; private set; }
    public string Cidade { get; private set; }
    public string Estado { get; private set; }

    protected Endereco() { }

    public Endereco(string logradouro, string cidade, string estado)
    {
        if (string.IsNullOrWhiteSpace(logradouro)) throw new ArgumentException("Logradouro (Endereço) é obrigatório.");
        if (string.IsNullOrWhiteSpace(cidade)) throw new ArgumentException("Cidade é obrigatória.");
        if (string.IsNullOrWhiteSpace(estado)) throw new ArgumentException("Estado é obrigatório.");

        Logradouro = logradouro;
        Cidade = cidade;
        Estado = estado;
    }
}
