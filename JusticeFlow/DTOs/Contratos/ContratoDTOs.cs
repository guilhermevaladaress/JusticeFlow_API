using JusticeFlow.Models;

namespace JusticeFlow.DTOs.Contratos;

public class CreateContratoRequest
{
    public int AdvogadoId { get; set; }
    public int ClienteId { get; set; }
    public DateOnly DataAssinatura { get; set; }
    public DateOnly? DataValidade { get; set; }
    public decimal ValorHonorarios { get; set; }
    public FormaHonorarios FormaPagamento { get; set; }
    public string? ConteudoBase64 { get; set; }
    public string? MimeType { get; set; }
}

public class UpdateContratoRequest
{
    public DateOnly? DataValidade { get; set; }
    public decimal? ValorHonorarios { get; set; }
    public StatusContrato? Status { get; set; }
}

public class ContratoResponse
{
    public int Id { get; set; }
    public string Advogado { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public DateOnly DataAssinatura { get; set; }
    public DateOnly? DataValidade { get; set; }
    public decimal ValorHonorarios { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class CreateHonorarioRequest
{
    public int ContratoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateOnly DataVencimento { get; set; }
}

public class UpdateHonorarioRequest
{
    public string? Descricao { get; set; }
    public decimal? Valor { get; set; }
    public DateOnly? DataVencimento { get; set; }
}

public class HonorarioResponse
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateOnly DataVencimento { get; set; }
    public DateOnly? DataPagamento { get; set; }
    public string Status { get; set; } = string.Empty;
}
