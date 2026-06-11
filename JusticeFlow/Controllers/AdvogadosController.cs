using JusticeFlow.Data;
using JusticeFlow.DTOs.Advogados;
using JusticeFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/advogados")]
[Authorize]
public class AdvogadosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<Usuario> _userManager;

    public AdvogadosController(AppDbContext context, UserManager<Usuario> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] CreateAdvogadoRequest req)
    {
        if (await _userManager.FindByEmailAsync(req.Email) != null)
            return Conflict(new { mensagem = "E-mail já cadastrado." });

        var usuario = new Usuario
        {
            UserName        = req.Email,
            Email           = req.Email,
            NomeCompleto    = req.NomeCompleto,
            CPF             = req.CPF,
            TelefoneContato = req.Telefone,
            EmailConfirmed  = true
        };

        var result = await _userManager.CreateAsync(usuario, req.Senha);
        if (!result.Succeeded)
            return BadRequest(new { mensagem = "Falha ao criar usuário.", erros = result.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(usuario, "Advogado");

        var advogado = new Advogado
        {
            UsuarioId     = usuario.Id,
            NumeroOAB     = req.NumeroOAB,
            UF            = req.UF,
            Especialidade = req.Especialidade,
            DataAdmissao  = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        _context.Advogados.Add(advogado);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = advogado.Id }, MapResponse(advogado));
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GetAll()
    {
        var advogados = await _context.Advogados
            .Include(a => a.Usuario)
            .AsNoTracking()
            .ToListAsync();

        return Ok(advogados.Select(MapResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var adv = await _context.Advogados
            .Include(a => a.Usuario)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (adv == null) return NotFound();
        return Ok(MapResponse(adv));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAdvogadoRequest req)
    {
        var adv = await _context.Advogados.Include(a => a.Usuario).FirstOrDefaultAsync(a => a.Id == id);
        if (adv == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Administrador");

        if (!isAdmin && adv.UsuarioId != userId)
            return Forbid();

        if (req.Especialidade != null) adv.Especialidade = req.Especialidade;
        if (req.Status.HasValue && isAdmin) adv.Status = req.Status.Value;
        if (req.Telefone != null) adv.Usuario.TelefoneContato = req.Telefone;

        await _context.SaveChangesAsync();
        return Ok(MapResponse(adv));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var adv = await _context.Advogados.FindAsync(id);
        if (adv == null) return NotFound();

        adv.Status = StatusAdvogado.Inativo;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static AdvogadoResponse MapResponse(Advogado a) => new()
    {
        Id           = a.Id,
        UsuarioId    = a.UsuarioId,
        NomeCompleto = a.Usuario.NomeCompleto,
        Email        = a.Usuario.Email ?? string.Empty,
        NumeroOAB    = a.NumeroOAB,
        UF           = a.UF,
        Especialidade = a.Especialidade,
        DataAdmissao = a.DataAdmissao,
        Status       = a.Status.ToString()
    };
}
