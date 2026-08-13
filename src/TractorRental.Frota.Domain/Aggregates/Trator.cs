using TractorRental.Frota.Domain.Enums;
using TractorRental.Frota.Domain.Events;

namespace TractorRental.Frota.Domain.Aggregates;

public class Trator
{
    public Guid Id { get; private set; }
    public string Modelo { get; private set; } = string.Empty;
    public double TemperaturaAtualMotor { get; private set; }
    public double PressaoAtualPneus { get; private set; }
    public double NivelCombustivel { get; private set; }
    public double NivelOleo { get; private set; }
    public double RotacaoMotor { get; private set; }
    public double Velocidade { get; private set; }
    public StatusTrator Status { get; private set; }

    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    protected Trator() { }

    public Trator(Guid id, string modelo)
    {
        Id = id;
        Modelo = modelo;
        Status = StatusTrator.Operacional;
    }

    public void Alugar()
    {
        if (Status != StatusTrator.Operacional)
            throw new InvalidOperationException("O equipamento só pode ser alugado se estiver com o status Operacional.");

        Status = StatusTrator.Alugado;
    }

    public void Desalugar()
    {
        if (Status == StatusTrator.Alugado)
            Status = StatusTrator.Operacional;
    }

    /// <summary>
    /// Atualiza os valores de sensor cacheados no agregado.
    /// Chamado pelo BC de Frota ao receber TelemetriaProcessadaIntegrationEvent.
    /// </summary>
    public void AtualizarSensores(double temperatura, double pressao, double combustivel, double oleo, double rotacao, double velocidade)
    {
        TemperaturaAtualMotor = temperatura;
        PressaoAtualPneus = pressao;
        NivelCombustivel = combustivel;
        NivelOleo = oleo;
        RotacaoMotor = rotacao;
        Velocidade = velocidade;
    }

    /// <summary>
    /// Coloca o trator em manutenção e dispara evento interno.
    /// </summary>
    public void RegistrarAlertaManutencao(string motivo)
    {
        Status = StatusTrator.EmManutencao;
        _domainEvents.Add(new AlertaGeradoEvent(Id, motivo, "ALTA", DateTime.UtcNow));
    }

    public void LimparEventos() => _domainEvents.Clear();
}
