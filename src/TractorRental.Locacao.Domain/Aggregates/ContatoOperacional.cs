namespace TractorRental.Locacao.Domain.Aggregates;

public class ContatoOperacional
{
    public string Nome { get; private set; }
    public string Telefone { get; private set; }
    public string Email { get; private set; }

    protected ContatoOperacional() { }

    public ContatoOperacional(string nome, string telefone, string email)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome do responsável é obrigatório.");
        if (string.IsNullOrWhiteSpace(telefone)) throw new ArgumentException("Telefone do responsável é obrigatório.");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("E-mail do responsável é obrigatório.");

        Nome = nome;
        Telefone = telefone;
        Email = email;
    }
}
