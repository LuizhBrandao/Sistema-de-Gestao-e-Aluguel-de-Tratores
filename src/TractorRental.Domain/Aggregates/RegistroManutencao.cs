namespace TractorRental.Domain.Aggregates;

public class RegistroManutencao
{
    public Guid Id { get; private set; }
    public Guid TratorId { get; private set; }
    public string DescricaoDefeito { get; private set; } = string.Empty;
    public DateTime DataEntrada { get; private set; }
    public DateTime? DataResolucao { get; private set; }

    protected RegistroManutencao() { }

    public RegistroManutencao(Guid id, Guid tratorId, string descricaoDefeito)
    {
        Id = id;
        TratorId = tratorId;
        DescricaoDefeito = descricaoDefeito;
        DataEntrada = DateTime.UtcNow;
    }

    public void FinalizarManutencao()
    {
        DataResolucao = DateTime.UtcNow;
    }
}