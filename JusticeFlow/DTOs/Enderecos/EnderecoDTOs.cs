namespace JusticeFlow.DTOs.Enderecos;

public class CreateEnderecoRequest
{
    public string CEP { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public bool Principal { get; set; } = false;
    // Logradouro, Bairro, Cidade, Estado preenchidos pelo ViaCEP
}

public class UpdateEnderecoRequest
{
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public bool? Principal { get; set; }
}

public class EnderecoResponse
{
    public int Id { get; set; }
    public string CEP { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool Principal { get; set; }
}

public class ViaCepResponse
{
    public string? Cep { get; set; }
    public string? Logradouro { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Localidade { get; set; }
    public string? Uf { get; set; }
    public bool Erro { get; set; }
}
