namespace JusticeFlow.Models;

public class TipoPrazo
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int DiasDefault { get; set; }
    public string? Descricao { get; set; }

    public ICollection<Prazo> Prazos { get; set; } = [];
}
