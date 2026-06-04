namespace JusticeFlow.Models;

public enum TipoCliente { PessoaFisica, PessoaJuridica }

public class Cliente
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public TipoCliente Tipo { get; set; } = TipoCliente.PessoaFisica;
    public string? CNPJ { get; set; }
    public string? RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public Usuario Usuario { get; set; } = null!;
    public ICollection<ProcessoCliente> ProcessosCliente { get; set; } = [];
    public ICollection<Contrato> Contratos { get; set; } = [];
}
