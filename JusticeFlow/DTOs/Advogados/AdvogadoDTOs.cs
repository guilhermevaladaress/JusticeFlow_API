using JusticeFlow.Models;

namespace JusticeFlow.DTOs.Advogados;

public class AdvogadoResponse
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NumeroOAB { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
    public string? Especialidade { get; set; }
    public DateOnly DataAdmissao { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UpdateAdvogadoRequest
{
    public string? Especialidade { get; set; }
    public StatusAdvogado? Status { get; set; }
    public string? Telefone { get; set; }
}
