using JusticeFlow.Data;
using JusticeFlow.DTOs.Clientes;
using JusticeFlow.Models;
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

    public ClientesController(AppDbContext context, UserManager<Usuario> userManager)
    {
        _context = context;
        _userManager = userManager;
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
