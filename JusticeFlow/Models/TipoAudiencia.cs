namespace JusticeFlow.Models;

public class TipoAudiencia
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    public ICollection<Audiencia> Audiencias { get; set; } = [];
}
