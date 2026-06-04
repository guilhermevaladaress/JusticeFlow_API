namespace JusticeFlow.Models;

public class Vara
{
    public int Id { get; set; }
    public int TribunalId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string? Competencia { get; set; }

    public Tribunal Tribunal { get; set; } = null!;
    public ICollection<Processo> Processos { get; set; } = [];
}
