using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JusticeFlow.Data;
using JusticeFlow.DTOs.Auth;
using JusticeFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(UserManager<Usuario> userManager, AppDbContext context, IConfiguration config)
    {
        _userManager = userManager;
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var rolesValidas = new[] { "Administrador", "Advogado", "Cliente" };
        if (!rolesValidas.Contains(req.Role))
            return BadRequest(new { mensagem = "Role inválida. Use: Administrador, Advogado ou Cliente." });

        if (await _userManager.FindByEmailAsync(req.Email) != null)
            return Conflict(new { mensagem = "E-mail já cadastrado." });

        var usuario = new Usuario
        {
            UserName        = req.Email,
            Email           = req.Email,
            NomeCompleto    = req.NomeCompleto,
            CPF             = req.CPF,
            TelefoneContato = req.Telefone,
            DataNascimento  = req.DataNascimento,
            EmailConfirmed  = true
        };

        var result = await _userManager.CreateAsync(usuario, req.Senha);
        if (!result.Succeeded)
            return BadRequest(new { mensagem = "Falha ao criar usuário.", erros = result.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(usuario, req.Role);

        if (req.Role == "Advogado")
        {
            if (string.IsNullOrWhiteSpace(req.NumeroOAB) || string.IsNullOrWhiteSpace(req.UF))
                return BadRequest(new { mensagem = "NumeroOAB e UF são obrigatórios para Advogado." });

            _context.Advogados.Add(new Advogado
            {
                UsuarioId    = usuario.Id,
                NumeroOAB    = req.NumeroOAB,
                UF           = req.UF,
                Especialidade = req.Especialidade,
                DataAdmissao = DateOnly.FromDateTime(DateTime.UtcNow)
            });
        }
        else if (req.Role == "Cliente")
        {
            _context.Clientes.Add(new Cliente
            {
                UsuarioId    = usuario.Id,
                DataCadastro = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
        return Ok(new { mensagem = "Usuário criado com sucesso." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var usuario = await _userManager.FindByEmailAsync(req.Email);
        if (usuario == null || !await _userManager.CheckPasswordAsync(usuario, req.Senha))
            return Unauthorized(new { mensagem = "Credenciais inválidas." });

        var roles = await _userManager.GetRolesAsync(usuario);
        var token = GerarToken(usuario, roles);

        return Ok(new LoginResponse
        {
            Token       = token,
            NomeCompleto = usuario.NomeCompleto,
            Email       = usuario.Email!,
            Role        = roles.FirstOrDefault() ?? string.Empty,
            Expiracao   = DateTime.UtcNow.AddHours(8)
        });
    }

    private string GerarToken(Usuario usuario, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id),
            new(JwtRegisteredClaimNames.Email, usuario.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id),
            new(ClaimTypes.Name, usuario.NomeCompleto)
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:   _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims:   claims,
            expires:  DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
