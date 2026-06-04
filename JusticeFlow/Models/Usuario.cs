using Microsoft.AspNetCore.Identity;

namespace JusticeFlow.Models;

public class Usuario : IdentityUser
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string? CPF { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? TelefoneContato { get; set; }

    public Advogado? Advogado { get; set; }
    public Cliente? Cliente { get; set; }
    public ICollection<Endereco> Enderecos { get; set; } = [];
    public ICollection<Notificacao> Notificacoes { get; set; } = [];
    public ICollection<Movimentacao> Movimentacoes { get; set; } = [];
    public ICollection<Documento> DocumentosUpload { get; set; } = [];
}
