using JusticeFlow.Models;

namespace JusticeFlow.DTOs.Clientes;

public class CnpjResponse
{
    public string? Cnpj { get; set; }
    public string? RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public string? SituacaoCadastral { get; set; }
    public string? NaturezaJuridica { get; set; }
    public string? Porte { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Municipio { get; set; }
    public string? Uf { get; set; }
    public string? Cep { get; set; }
}

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
    public string? CEP { get; set; }
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}

public class CreateClienteRequest
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoCliente Tipo { get; set; } = TipoCliente.PessoaFisica;
    public string? CPF { get; set; }
    public string? Telefone { get; set; }
    public string? CNPJ { get; set; }
    public string? RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public string? CEP { get; set; }
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
}

public class UpdateClienteRequest
{
    public TipoCliente? Tipo { get; set; }
    public string? CNPJ { get; set; }
    public string? RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public string? Telefone { get; set; }
}
