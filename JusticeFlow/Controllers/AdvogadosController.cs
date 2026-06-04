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
