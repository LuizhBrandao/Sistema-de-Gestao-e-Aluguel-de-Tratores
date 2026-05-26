using TractorRental.Domain.Enums;
using TractorRental.Domain.Events;

namespace TractorRental.Domain.Aggregates;

public class Trator
{
    public Guid Id { get; private set; }
    public string Modelo { get; private set; } = string.Empty;
    public double TemperaturaAtualMotor { get; private set; }
    public double PressaoAtualPneus { get; private set; }
    public double NivelCombustivel { get; private set; }
    public StatusTrator Status { get; private set; }

    // Lista para acumular eventos disparados pela entidade
    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    protected Trator() { } // Construtor pro EF Core

    public Trator(Guid id, string modelo)
    {
        Id = id;
        Modelo = modelo;
        Status = StatusTrator.Operacional;
    }

    // Regra de Negócio: Processa a telemetria que vem do Worker/RabbitMQ
    public void ProcessarLeituraSensores(double temperatura, double pressao, double combustivel)
    {
        TemperaturaAtualMotor = temperatura;
        PressaoAtualPneus = pressao;
        NivelCombustivel = combustivel;

        _domainEvents.Add(new LeituraRecebidaEvent(Id, temperatura, pressao, combustivel, DateTime.UtcNow));
    }

    // Regra de Negócio: Alerta de risco (Policy)
    public void RegistrarAlertaManutencao(string motivo)
    {
        Status = StatusTrator.EmManutencao;
        _domainEvents.Add(new AlertaGeradoEvent(Id, motivo, "ALTA", DateTime.UtcNow));
    }

    public void LimparEventos() => _domainEvents.Clear();
}