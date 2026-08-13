namespace TractorRental.Locacao.Domain.Aggregates;

public class Cliente
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Documento { get; private set; } = string.Empty;

    protected Cliente() { }

    public Cliente(Guid id, string nome, string documento)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("O nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(documento)) throw new ArgumentException("O documento é obrigatório.");

        Id = id;
        Nome = nome;
        Documento = documento;
    }

    public void AtualizarDados(string nome, string documento)
    {
        Nome = nome;
        Documento = documento;
    }
}
