using System.Text.Json.Serialization;

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

public class CadastroTratorForm
{
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int? AnoFabricacao { get; set; } = DateTime.Now.Year;
    public int? PotenciaCv { get; set; }
    public double? HorimetroInicial { get; set; } = 0;
    public string NumeroSerie { get; set; } = string.Empty;
}

public class ClienteDto
{
    public Guid Id { get; set; }
    public string DocumentoNumero { get; set; } = string.Empty;
    public object? DocumentoTipo { get; set; }
    public string RazaoSocialOuNome { get; set; } = string.Empty;
    public string? InscricaoEstadual { get; set; }
    public string EmailFaturamento { get; set; } = string.Empty;
    public string ContatoNome { get; set; } = string.Empty;
    public string ContatoTelefone { get; set; } = string.Empty;
    public string ContatoEmail { get; set; } = string.Empty;
    public string EnderecoLogradouro { get; set; } = string.Empty;
    public string EnderecoCidade { get; set; } = string.Empty;
    public string EnderecoEstado { get; set; } = string.Empty;

    public string TipoDescricao => DocumentoTipo?.ToString() switch
    {
        "1" or "Fisica" or "PessoaFisica" => "Pessoa Física",
        "2" or "Juridica" or "PessoaJuridica" => "Pessoa Jurídica",
        _ => DocumentoTipo?.ToString() ?? "Pessoa Jurídica"
    };
}

public class CadastroClienteForm
{
    public string TipoPessoa { get; set; } = "Juridica";
    public string Documento { get; set; } = string.Empty;
    public string RazaoSocialOuNome { get; set; } = string.Empty;
    public string? InscricaoEstadual { get; set; }
    public string EmailFaturamento { get; set; } = string.Empty;
    public string NomeResponsavelOperacional { get; set; } = string.Empty;
    public string TelefoneOperacional { get; set; } = string.Empty;
    public string EnderecoOperacao { get; set; } = string.Empty;
    public string CidadeOperacao { get; set; } = string.Empty;
    public string EstadoOperacao { get; set; } = string.Empty;
}

public class ContratoDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public Guid TratorId { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public decimal ValorHora { get; set; }
    public object? Status { get; set; }

    public string StatusFormatado => Status?.ToString() switch
    {
        "1" or "Ativo" => "Ativo",
        "2" or "Finalizado" => "Finalizado",
        _ => Status?.ToString() ?? "Ativo"
    };
}

public class CadastroContratoForm
{
    public Guid? ClienteId { get; set; }
    public Guid? TratorId { get; set; }
    public decimal? ValorHora { get; set; } = 150.00m;
}

public class AlertaDto
{
    public Guid TratorId { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public double? Temperatura { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
