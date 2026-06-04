namespace JusticeFlow.Models;

public class Endereco
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool Principal { get; set; } = false;

    public Usuario Usuario { get; set; } = null!;
}
