using JusticeFlow.Models;

namespace JusticeFlow.DTOs.Prazos;

public class CreatePrazoRequest
{
    public int ProcessoId { get; set; }
    public int TipoPrazoId { get; set; }
    public int? AdvogadoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataVencimento { get; set; }
    public string? Observacoes { get; set; }
}

public class UpdatePrazoRequest
{
    public string? Descricao { get; set; }
    public DateTime? DataVencimento { get; set; }
    public int? AdvogadoId { get; set; }
    public string? Observacoes { get; set; }
}

public class PrazoResponse
{
    public int Id { get; set; }
    public int ProcessoId { get; set; }
    public string NumeroProcesso { get; set; } = string.Empty;
    public string ProcessoTitulo { get; set; } = string.Empty;
    public int TipoPrazoId { get; set; }
    public string TipoPrazoNome { get; set; } = string.Empty;
    public string? AdvogadoNome { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataVencimento { get; set; }
    public DateTime? DataCumprimento { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public bool Vencendo { get; set; }
}
