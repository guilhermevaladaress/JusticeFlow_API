using JusticeFlow.Models;

namespace JusticeFlow.DTOs.Clientes;

public class ClienteResponse
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? CPF { get; set; }
    public string? Telefone { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string? CNPJ { get; set; }
    public string? RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public DateTime DataCadastro { get; set; }
}

public class UpdateClienteRequest
{
    public TipoCliente? Tipo { get; set; }
    public string? CNPJ { get; set; }
    public string? RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public string? Telefone { get; set; }
}
