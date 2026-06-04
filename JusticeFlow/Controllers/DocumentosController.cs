using JusticeFlow.Data;
using JusticeFlow.DTOs.Documentos;
using JusticeFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/documentos")]
[Authorize]
public class DocumentosController : ControllerBase
{
    private readonly AppDbContext _context;

    public DocumentosController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? processoId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Administrador");

        IQueryable<Documento> query = _context.Documentos
            .Include(d => d.Processo)
            .Include(d => d.TipoDocumento)
            .Include(d => d.UploadUsuario)
            .Where(d => d.Status == StatusDocumento.Ativo)
            .AsNoTracking();

        if (processoId.HasValue)
            query = query.Where(d => d.ProcessoId == processoId.Value);

        if (!isAdmin)
        {
            if (User.IsInRole("Advogado"))
            {
                var adv = await _context.Advogados.FirstOrDefaultAsync(a => a.UsuarioId == userId);
                if (adv != null)
                    query = query.Where(d => d.Processo.ProcessosAdvogado.Any(pa => pa.AdvogadoId == adv.Id && pa.Ativo));
            }
            else
            {
                var cli = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
                if (cli != null)
                    query = query.Where(d => d.Processo.ProcessosCliente.Any(pc => pc.ClienteId == cli.Id));
            }
        }

        var list = await query.ToListAsync();
        return Ok(list.Select(MapResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var d = await _context.Documentos
            .Include(d => d.Processo)
            .Include(d => d.TipoDocumento)
            .Include(d => d.UploadUsuario)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (d == null) return NotFound();
        return Ok(MapResponse(d));
    }

    [HttpGet("{id}/arquivo")]
    public async Task<IActionResult> GetArquivo(int id)
    {
        var d = await _context.Documentos.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        if (d == null) return NotFound();

        return Ok(new DocumentoArquivoResponse
        {
            NomeArquivo    = d.NomeArquivo,
            ConteudoBase64 = d.ConteudoBase64,
            MimeType       = d.MimeType
        });
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> Create([FromBody] CreateDocumentoRequest req)
    {
        if (!await _context.Processos.AnyAsync(p => p.Id == req.ProcessoId))
            return BadRequest(new { mensagem = "Processo não encontrado." });

        if (!await _context.TiposDocumento.AnyAsync(t => t.Id == req.TipoDocumentoId))
            return BadRequest(new { mensagem = "TipoDocumento inválido." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var doc = new Documento
        {
            ProcessoId      = req.ProcessoId,
            TipoDocumentoId = req.TipoDocumentoId,
            UploadUsuarioId = userId,
            Nome            = req.Nome,
            Descricao       = req.Descricao,
            NomeArquivo     = req.NomeArquivo,
            ConteudoBase64  = req.ConteudoBase64,
            MimeType        = req.MimeType
        };

        _context.Documentos.Add(doc);

        _context.Movimentacoes.Add(new Movimentacao
        {
            ProcessoId = req.ProcessoId,
            UsuarioId  = userId,
            Descricao  = $"Documento '{req.Nome}' adicionado ao processo.",
            Tipo       = TipoMovimentacao.Documento
        });

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = doc.Id }, new { doc.Id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador,Advogado")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentoRequest req)
    {
        var doc = await _context.Documentos.FindAsync(id);
        if (doc == null) return NotFound();

        if (req.Nome != null)      doc.Nome = req.Nome;
        if (req.Descricao != null) doc.Descricao = req.Descricao;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var doc = await _context.Documentos.FindAsync(id);
        if (doc == null) return NotFound();

        doc.Status = StatusDocumento.Arquivado;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static DocumentoResponse MapResponse(Documento d) => new()
    {
        Id             = d.Id,
        ProcessoId     = d.ProcessoId,
        NumeroProcesso = d.Processo?.NumeroProcesso ?? string.Empty,
        TipoDocumento  = d.TipoDocumento?.Nome ?? string.Empty,
        Nome           = d.Nome,
        Descricao      = d.Descricao,
        NomeArquivo    = d.NomeArquivo,
        MimeType       = d.MimeType,
        DataUpload     = d.DataUpload,
        UploadPor      = d.UploadUsuario?.NomeCompleto ?? string.Empty,
        Status         = d.Status.ToString()
    };
}
