namespace JusticeFlow.Models;

public enum StatusPrazo { Pendente, Cumprido, Vencido }

public class Prazo
{
    public int Id { get; set; }
    public int ProcessoId { get; set; }
    public int TipoPrazoId { get; set; }
    public int? AdvogadoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataVencimento { get; set; }
    public DateTime? DataCumprimento { get; set; }
    public StatusPrazo Status { get; set; } = StatusPrazo.Pendente;
    public string? Observacoes { get; set; }

    public Processo Processo { get; set; } = null!;
    public TipoPrazo TipoPrazo { get; set; } = null!;
    public Advogado? Advogado { get; set; }
}
