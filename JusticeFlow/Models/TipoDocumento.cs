namespace JusticeFlow.Models;

public class TipoDocumento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    public ICollection<Documento> Documentos { get; set; } = [];
}
