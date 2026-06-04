namespace JusticeFlow.Models;

public class ConfiguracaoEscritorio
{
    public int Id { get; set; }
    public string NomeEscritorio { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? LogoBase64 { get; set; }
    public string? LogoMimeType { get; set; }
}
