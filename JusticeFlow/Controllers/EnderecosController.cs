using JusticeFlow.Data;
using JusticeFlow.DTOs.Enderecos;
using JusticeFlow.Models;
using JusticeFlow.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JusticeFlow.Controllers;

[ApiController]
[Route("api/enderecos")]
[Authorize]
public class EnderecosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IViaCepService _viaCep;

    public EnderecosController(AppDbContext context, IViaCepService viaCep)
    {
        _context = context;
        _viaCep = viaCep;
    }

    [HttpGet("cep/{cep}")]
    public async Task<IActionResult> ConsultarCep(string cep)
    {
        var resultado = await _viaCep.ConsultarCepAsync(cep);
        if (resultado == null)
            return NotFound(new { mensagem = "CEP não encontrado." });

        return Ok(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> GetMeus()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var enderecos = await _context.Enderecos
            .Where(e => e.UsuarioId == userId)
            .AsNoTracking()
            .ToListAsync();

        return Ok(enderecos.Select(MapResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEnderecoRequest req)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var dados = await _viaCep.ConsultarCepAsync(req.CEP);
        if (dados == null)
            return BadRequest(new { mensagem = "CEP inválido ou não encontrado." });

        if (req.Principal)
        {
            var anteriores = await _context.Enderecos
                .Where(e => e.UsuarioId == userId && e.Principal)
                .ToListAsync();
            anteriores.ForEach(e => e.Principal = false);
        }

        var endereco = new Endereco
        {
            UsuarioId   = userId,
            CEP         = dados.Cep ?? req.CEP,
            Logradouro  = dados.Logradouro ?? string.Empty,
            Bairro      = dados.Bairro ?? string.Empty,
            Cidade      = dados.Localidade ?? string.Empty,
            Estado      = dados.Uf ?? string.Empty,
            Numero      = req.Numero,
            Complemento = req.Complemento,
            Principal   = req.Principal
        };

        _context.Enderecos.Add(endereco);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMeus), MapResponse(endereco));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEnderecoRequest req)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var end = await _context.Enderecos.FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == userId);
        if (end == null) return NotFound();

        if (req.Numero != null) end.Numero = req.Numero;
        if (req.Complemento != null) end.Complemento = req.Complemento;

        if (req.Principal == true)
        {
            var outros = await _context.Enderecos
                .Where(e => e.UsuarioId == userId && e.Principal && e.Id != id)
                .ToListAsync();
            outros.ForEach(e => e.Principal = false);
            end.Principal = true;
        }

        await _context.SaveChangesAsync();
        return Ok(MapResponse(end));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var end = await _context.Enderecos.FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == userId);
        if (end == null) return NotFound();

        _context.Enderecos.Remove(end);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static EnderecoResponse MapResponse(Endereco e) => new()
    {
        Id          = e.Id,
        CEP         = e.CEP,
        Logradouro  = e.Logradouro,
        Numero      = e.Numero,
        Complemento = e.Complemento,
        Bairro      = e.Bairro,
        Cidade      = e.Cidade,
        Estado      = e.Estado,
        Principal   = e.Principal
    };
}
