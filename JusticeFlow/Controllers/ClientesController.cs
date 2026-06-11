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
            .Include(c => c.Usuario)
            .AsNoTracking()
            .ToListAsync();

        return Ok(clientes.Select(MapResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cli = await _context.Clientes
            .Include(c => c.Usuario)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cli == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Administrador") || User.IsInRole("Advogado");

        if (!isAdmin && cli.UsuarioId != userId)
            return Forbid();

        return Ok(MapResponse(cli));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteRequest req)
    {
        var cli = await _context.Clientes.Include(c => c.Usuario).FirstOrDefaultAsync(c => c.Id == id);
        if (cli == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Administrador") || User.IsInRole("Advogado");

        if (!isAdmin && cli.UsuarioId != userId)
            return Forbid();

        if (req.Tipo.HasValue)    cli.Tipo = req.Tipo.Value;
        if (req.CNPJ != null)     cli.CNPJ = req.CNPJ;
        if (req.RazaoSocial != null)  cli.RazaoSocial = req.RazaoSocial;
        if (req.NomeFantasia != null) cli.NomeFantasia = req.NomeFantasia;
        if (req.Telefone != null) cli.Usuario.TelefoneContato = req.Telefone;

        await _context.SaveChangesAsync();
        return Ok(MapResponse(cli));
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

    private static ClienteResponse MapResponse(Cliente c) => new()
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
        DataCadastro = c.DataCadastro
    };
}
