namespace JusticeFlow.Models;

public enum StatusAudiencia { Agendada, Realizada, Cancelada, Adiada }

public class Audiencia
{
    public int Id { get; set; }
    public int ProcessoId { get; set; }
    public int TipoAudienciaId { get; set; }
    public DateTime DataHora { get; set; }
    public string? Local { get; set; }
    public string? LinkVirtual { get; set; }
    public StatusAudiencia Status { get; set; } = StatusAudiencia.Agendada;
    public string? Observacoes { get; set; }

    public Processo Processo { get; set; } = null!;
    public TipoAudiencia TipoAudiencia { get; set; } = null!;
}
