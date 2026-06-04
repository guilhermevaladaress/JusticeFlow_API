namespace JusticeFlow.Models;

public enum TipoTribunal { Superior, Federal, Estadual, Trabalhista, Eleitoral }

public class Tribunal
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public TipoTribunal Tipo { get; set; }
    public string? Estado { get; set; }
    public string? CodigoCNJ { get; set; }

    public ICollection<Vara> Varas { get; set; } = [];
    public ICollection<Processo> Processos { get; set; } = [];
}
