namespace TractorRental.BlazorFrontend.Models;

public class TratorDto
{
    public Guid Id { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double TemperaturaAtualMotor { get; set; }
    public double PressaoAtualPneus { get; set; }
    public double NivelCombustivel { get; set; }
    public double NivelOleo { get; set; }
    public double RotacaoMotor { get; set; }
    public double Velocidade { get; set; }
}

public class AlertaDto
{
    public Guid TratorId { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public double? Temperatura { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class CadastroTratorForm
{
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int? AnoFabricacao { get; set; }
    public int? PotenciaCv { get; set; }
    public double? HorimetroInicial { get; set; }
    public string NumeroSerie { get; set; } = string.Empty;
}
