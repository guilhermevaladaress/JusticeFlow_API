using JusticeFlow.Data;
using JusticeFlow.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/notificacoes")]
[Authorize]
public class NotificacoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public NotificacoesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetMinhas()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var list = await _context.Notificacoes
            .Where(n => n.UsuarioId == userId)
            .OrderByDescending(n => n.DataCriacao)
            .AsNoTracking()
            .ToListAsync();

        return Ok(list.Select(MapResponse));
    }

    [HttpGet("nao-lidas")]
    public async Task<IActionResult> GetNaoLidas()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var list = await _context.Notificacoes
            .Where(n => n.UsuarioId == userId && !n.Lida)
            .OrderByDescending(n => n.DataCriacao)
            .AsNoTracking()
            .ToListAsync();

        return Ok(list.Select(MapResponse));
    }

    [HttpPut("{id}/ler")]
    public async Task<IActionResult> MarcarLida(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var n = await _context.Notificacoes.FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == userId);
        if (n == null) return NotFound();

        n.Lida = true;
        n.DataLeitura = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("ler-todas")]
    public async Task<IActionResult> MarcarTodasLidas()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var naoLidas = await _context.Notificacoes
            .Where(n => n.UsuarioId == userId && !n.Lida)
            .ToListAsync();

        var agora = DateTime.UtcNow;
        naoLidas.ForEach(n => { n.Lida = true; n.DataLeitura = agora; });

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static NotificacaoResponse MapResponse(Models.Notificacao n) => new()
    {
        Id          = n.Id,
        Titulo      = n.Titulo,
        Mensagem    = n.Mensagem,
        Tipo        = n.Tipo.ToString(),
        Lida        = n.Lida,
        DataCriacao = n.DataCriacao,
        ProcessoId  = n.ProcessoId
    };
}
