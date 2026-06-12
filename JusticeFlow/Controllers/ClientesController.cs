using JusticeFlow.Data;
using JusticeFlow.DTOs.Clientes;
using JusticeFlow.Models;
using JusticeFlow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<Usuario> _userManager;
    private readonly IBrasilApiService _brasilApi;

    public ClientesController(AppDbContext context, UserManager<Usuario> userManager, IBrasilApiService brasilApi)
    {
        _context = context;
        _userManager = userManager;
        _brasilApi = brasilApi;
    }

    /// <summary>Consulta dados de empresa via CNPJ na BrasilAPI (público).</summary>
    [HttpGet("cnpj/{cnpj}")]
    [AllowAnonymous]
    public async Task<IActionResult> ConsultarCnpj(string cnpj)
    {
        try
        {
            var resultado = await _brasilApi.ConsultarCnpjAsync(cnpj);
            if (resultado == null)
                return NotFound(new { mensagem = "CNPJ não encontrado ou inválido." });

            return Ok(resultado);
        }
        catch (BrasilApiRateLimitException ex)
        {
            return StatusCode(429, new { mensagem = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> GetAll()
    {
        var clientes = await _context.Clientes
            .Include(c => c.Usuario).ThenInclude(u => u.Enderecos)
            .AsNoTracking()
            .ToListAsync();

        return Ok(clientes.Select(c => MapResponse(c, PrincipalEndereco(c))));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cli = await _context.Clientes
            .Include(c => c.Usuario).ThenInclude(u => u.Enderecos)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cli == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Administrador") || User.IsInRole("Advogado");

        if (!isAdmin && cli.UsuarioId != userId)
            return Forbid();

        return Ok(MapResponse(cli, PrincipalEndereco(cli)));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] CreateClienteRequest req)
    {
        if (await _userManager.FindByEmailAsync(req.Email) != null)
            return Conflict(new { mensagem = "E-mail já cadastrado." });

        var rnd = Guid.NewGuid().ToString("N")[..8];
        var senhaTemp = $"Jf@{rnd}";

        var usuario = new Usuario
        {
            UserName        = req.Email,
            Email           = req.Email,
            NomeCompleto    = req.NomeCompleto,
            CPF             = req.CPF,
            TelefoneContato = req.Telefone,
            EmailConfirmed  = true
        };

        var result = await _userManager.CreateAsync(usuario, senhaTemp);
        if (!result.Succeeded)
            return BadRequest(new { mensagem = "Erro ao criar usuário.", erros = result.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(usuario, "Cliente");

        var cliente = new Cliente
        {
            UsuarioId    = usuario.Id,
            Tipo         = req.Tipo,
            CNPJ         = req.CNPJ,
            RazaoSocial  = req.RazaoSocial,
            NomeFantasia = req.NomeFantasia,
            DataCadastro = DateTime.UtcNow
        };
        _context.Clientes.Add(cliente);

        Endereco? endereco = null;
        if (!string.IsNullOrWhiteSpace(req.CEP))
        {
            endereco = new Endereco
            {
                UsuarioId   = usuario.Id,
                CEP         = req.CEP!,
                Logradouro  = req.Logradouro ?? string.Empty,
                Numero      = req.Numero ?? string.Empty,
                Complemento = req.Complemento,
                Bairro      = req.Bairro ?? string.Empty,
                Cidade      = req.Cidade ?? string.Empty,
                Estado      = req.Estado ?? string.Empty,
                Principal   = true
            };
            _context.Enderecos.Add(endereco);
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, new
        {
            cliente         = MapResponse(cliente, endereco),
            senhaTemporaria = senhaTemp
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteRequest req)
    {
        var cli = await _context.Clientes
            .Include(c => c.Usuario).ThenInclude(u => u.Enderecos)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (cli == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Administrador") || User.IsInRole("Advogado");

        if (!isAdmin && cli.UsuarioId != userId)
            return Forbid();

        if (req.Tipo.HasValue)        cli.Tipo = req.Tipo.Value;
        if (req.CNPJ != null)         cli.CNPJ = req.CNPJ;
        if (req.RazaoSocial != null)  cli.RazaoSocial = req.RazaoSocial;
        if (req.NomeFantasia != null)  cli.NomeFantasia = req.NomeFantasia;
        if (req.Telefone != null)      cli.Usuario.TelefoneContato = req.Telefone;

        await _context.SaveChangesAsync();
        return Ok(MapResponse(cli, PrincipalEndereco(cli)));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var cli = await _context.Clientes.Include(c => c.Usuario).FirstOrDefaultAsync(c => c.Id == id);
        if (cli == null) return NotFound();

        _context.Clientes.Remove(cli);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static Endereco? PrincipalEndereco(Cliente c) =>
        c.Usuario.Enderecos.FirstOrDefault(e => e.Principal)
        ?? c.Usuario.Enderecos.FirstOrDefault();

    private static ClienteResponse MapResponse(Cliente c, Endereco? end = null) => new()
    {
        Id           = c.Id,
        UsuarioId    = c.UsuarioId,
        NomeCompleto = c.Usuario.NomeCompleto,
        Email        = c.Usuario.Email ?? string.Empty,
        CPF          = c.Usuario.CPF,
        Telefone     = c.Usuario.TelefoneContato,
        Tipo         = c.Tipo.ToString(),
        CNPJ         = c.CNPJ,
        RazaoSocial  = c.RazaoSocial,
        NomeFantasia = c.NomeFantasia,
        DataCadastro = c.DataCadastro,
        CEP          = end?.CEP,
        Logradouro   = end?.Logradouro,
        Numero       = end?.Numero,
        Complemento  = end?.Complemento,
        Bairro       = end?.Bairro,
        Cidade       = end?.Cidade,
        Estado       = end?.Estado,
    };
}
