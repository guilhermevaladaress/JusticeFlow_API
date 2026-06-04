namespace JusticeFlow.Models;

public enum TipoVinculoAdvogado { Titular, Substituto }

public class ProcessoAdvogado
{
    public int ProcessoId { get; set; }
    public int AdvogadoId { get; set; }
    public DateTime DataVinculo { get; set; } = DateTime.UtcNow;
    public TipoVinculoAdvogado TipoVinculo { get; set; } = TipoVinculoAdvogado.Titular;
    public bool Ativo { get; set; } = true;

    public Processo Processo { get; set; } = null!;
    public Advogado Advogado { get; set; } = null!;
}
