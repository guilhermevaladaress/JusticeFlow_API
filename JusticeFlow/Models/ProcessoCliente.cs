namespace JusticeFlow.Models;

public enum TipoParteProcesso { Autor, Reu, Terceiro }

public class ProcessoCliente
{
    public int ProcessoId { get; set; }
    public int ClienteId { get; set; }
    public DateTime DataVinculo { get; set; } = DateTime.UtcNow;
    public TipoParteProcesso TipoParte { get; set; } = TipoParteProcesso.Autor;

    public Processo Processo { get; set; } = null!;
    public Cliente Cliente { get; set; } = null!;
}
