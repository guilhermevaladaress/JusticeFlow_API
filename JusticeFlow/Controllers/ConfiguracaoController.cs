using JusticeFlow.Data;
using JusticeFlow.DTOs.Shared;
using JusticeFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/configuracao")]
[Authorize(Roles = "Administrador")]
public class ConfiguracaoController : ControllerBase
{
    private readonly AppDbContext _context;

    public ConfiguracaoController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var cfg = await _context.ConfiguracoesEscritorio.AsNoTracking().FirstOrDefaultAsync();
        if (cfg == null) return NotFound();

        return Ok(new ConfiguracaoEscritorioResponse
        {
            Id             = cfg.Id,
            NomeEscritorio = cfg.NomeEscritorio,
            CNPJ           = cfg.CNPJ,
            Telefone       = cfg.Telefone,
            Email          = cfg.Email,
            LogoBase64     = cfg.LogoBase64,
            LogoMimeType   = cfg.LogoMimeType
        });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateConfiguracaoRequest req)
    {
        var cfg = await _context.ConfiguracoesEscritorio.FirstOrDefaultAsync();

        if (cfg == null)
        {
            cfg = new ConfiguracaoEscritorio();
            _context.ConfiguracoesEscritorio.Add(cfg);
        }

        if (req.NomeEscritorio != null) cfg.NomeEscritorio = req.NomeEscritorio;
        if (req.CNPJ != null)           cfg.CNPJ = req.CNPJ;
        if (req.Telefone != null)       cfg.Telefone = req.Telefone;
        if (req.Email != null)          cfg.Email = req.Email;
        if (req.LogoBase64 != null)     cfg.LogoBase64 = req.LogoBase64;
        if (req.LogoMimeType != null)   cfg.LogoMimeType = req.LogoMimeType;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
