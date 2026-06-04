namespace JusticeFlow.DTOs.Auth;

public class RegisterRequest
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Role { get; set; } = "Cliente";
    public string? CPF { get; set; }
    public string? Telefone { get; set; }
    public DateTime? DataNascimento { get; set; }
    // Advogado extra fields
    public string? NumeroOAB { get; set; }
    public string? UF { get; set; }
    public string? Especialidade { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime Expiracao { get; set; }
}
