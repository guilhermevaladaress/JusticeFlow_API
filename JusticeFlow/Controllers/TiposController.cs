using JusticeFlow.Data;
using JusticeFlow.DTOs.Shared;
using JusticeFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JusticeFlow.Controllers;

// TipoProcesso
[ApiController]
[Route("api/tipos-processo")]
[Authorize]
public class TiposProcessoController : ControllerBase
{
    private readonly AppDbContext _context;
    public TiposProcessoController(AppDbContext context) => _context = context;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() =>
        Ok((await _context.TiposProcesso.AsNoTracking().ToListAsync())
            .Select(t => new TipoResponse { Id = t.Id, Nome = t.Nome, Descricao = t.Descricao }));

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] CreateTipoRequest req)
    {
        var tipo = new TipoProcesso { Nome = req.Nome, Descricao = req.Descricao };
        _context.TiposProcesso.Add(tipo);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = tipo.Id }, new TipoResponse { Id = tipo.Id, Nome = tipo.Nome, Descricao = tipo.Descricao });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTipoRequest req)
    {
        var t = await _context.TiposProcesso.FindAsync(id);
        if (t == null) return NotFound();
        if (req.Nome != null)      t.Nome = req.Nome;
        if (req.Descricao != null) t.Descricao = req.Descricao;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _context.TiposProcesso.FindAsync(id);
        if (t == null) return NotFound();
        _context.TiposProcesso.Remove(t);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

// TipoAudiencia
[ApiController]
[Route("api/tipos-audiencia")]
[Authorize]
public class TiposAudienciaController : ControllerBase
{
    private readonly AppDbContext _context;
    public TiposAudienciaController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok((await _context.TiposAudiencia.AsNoTracking().ToListAsync())
            .Select(t => new TipoResponse { Id = t.Id, Nome = t.Nome, Descricao = t.Descricao }));

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] CreateTipoRequest req)
    {
        var tipo = new TipoAudiencia { Nome = req.Nome, Descricao = req.Descricao };
        _context.TiposAudiencia.Add(tipo);
        await _context.SaveChangesAsync();
        return Ok(new TipoResponse { Id = tipo.Id, Nome = tipo.Nome, Descricao = tipo.Descricao });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTipoRequest req)
    {
        var t = await _context.TiposAudiencia.FindAsync(id);
        if (t == null) return NotFound();
        if (req.Nome != null)      t.Nome = req.Nome;
        if (req.Descricao != null) t.Descricao = req.Descricao;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _context.TiposAudiencia.FindAsync(id);
        if (t == null) return NotFound();
        _context.TiposAudiencia.Remove(t);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

// TipoPrazo
[ApiController]
[Route("api/tipos-prazo")]
[Authorize]
public class TiposPrazoController : ControllerBase
{
    private readonly AppDbContext _context;
    public TiposPrazoController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok((await _context.TiposPrazo.AsNoTracking().ToListAsync())
            .Select(t => new TipoResponse { Id = t.Id, Nome = t.Nome, Descricao = t.Descricao }));

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] CreateTipoRequest req)
    {
        var tipo = new TipoPrazo { Nome = req.Nome, Descricao = req.Descricao };
        _context.TiposPrazo.Add(tipo);
        await _context.SaveChangesAsync();
        return Ok(new TipoResponse { Id = tipo.Id, Nome = tipo.Nome, Descricao = tipo.Descricao });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTipoRequest req)
    {
        var t = await _context.TiposPrazo.FindAsync(id);
        if (t == null) return NotFound();
        if (req.Nome != null)      t.Nome = req.Nome;
        if (req.Descricao != null) t.Descricao = req.Descricao;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _context.TiposPrazo.FindAsync(id);
        if (t == null) return NotFound();
        _context.TiposPrazo.Remove(t);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

// TipoDocumento
[ApiController]
[Route("api/tipos-documento")]
[Authorize]
public class TiposDocumentoController : ControllerBase
{
    private readonly AppDbContext _context;
    public TiposDocumentoController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok((await _context.TiposDocumento.AsNoTracking().ToListAsync())
            .Select(t => new TipoResponse { Id = t.Id, Nome = t.Nome, Descricao = t.Descricao }));

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] CreateTipoRequest req)
    {
        var tipo = new TipoDocumento { Nome = req.Nome, Descricao = req.Descricao };
        _context.TiposDocumento.Add(tipo);
        await _context.SaveChangesAsync();
        return Ok(new TipoResponse { Id = tipo.Id, Nome = tipo.Nome, Descricao = tipo.Descricao });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTipoRequest req)
    {
        var t = await _context.TiposDocumento.FindAsync(id);
        if (t == null) return NotFound();
        if (req.Nome != null)      t.Nome = req.Nome;
        if (req.Descricao != null) t.Descricao = req.Descricao;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _context.TiposDocumento.FindAsync(id);
        if (t == null) return NotFound();
        _context.TiposDocumento.Remove(t);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
