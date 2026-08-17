using TractorRental.Frota.Domain.Enums;
using TractorRental.Frota.Domain.Events;

namespace TractorRental.Frota.Domain.Aggregates;

public class Trator
{
    public Guid Id { get; private set; }
    public MarcaTrator Marca { get; private set; }
    public string Modelo { get; private set; } = string.Empty;
    public int AnoFabricacao { get; private set; }
    public int PotenciaCv { get; private set; }
    public double HorimetroInicial { get; private set; }
    public string NumeroSerie { get; private set; } = string.Empty;
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

    public Trator(Guid id, string marca, string modelo, int anoFabricacao,
                  int potenciaCv, double horimetroInicial, string numeroSerie)
    {
        Id = id;
        Marca = Enum.TryParse<MarcaTrator>(marca, true, out var parsedMarca) 
            ? parsedMarca 
            : throw new ArgumentException($"Marca inválida: '{marca}'. Valores aceitos: {string.Join(", ", Enum.GetNames<MarcaTrator>())}");
        Modelo = !string.IsNullOrWhiteSpace(modelo)
            ? modelo
            : throw new ArgumentException("Modelo é obrigatório.", nameof(modelo));
        AnoFabricacao = anoFabricacao > 1950 && anoFabricacao <= DateTime.UtcNow.Year + 1
            ? anoFabricacao
            : throw new ArgumentOutOfRangeException(nameof(anoFabricacao), "Ano de fabricação deve estar entre 1950 e o próximo ano.");
        PotenciaCv = potenciaCv > 0
            ? potenciaCv
            : throw new ArgumentOutOfRangeException(nameof(potenciaCv), "Potência deve ser positiva.");
        HorimetroInicial = horimetroInicial >= 0
            ? horimetroInicial
            : throw new ArgumentOutOfRangeException(nameof(horimetroInicial), "Horímetro não pode ser negativo.");
        NumeroSerie = !string.IsNullOrWhiteSpace(numeroSerie)
            ? numeroSerie
            : throw new ArgumentException("Número de Série (PIN) é obrigatório.", nameof(numeroSerie));
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
