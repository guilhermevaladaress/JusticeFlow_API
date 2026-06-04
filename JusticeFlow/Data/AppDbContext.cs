using JusticeFlow.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JusticeFlow.Data;

public class AppDbContext : IdentityDbContext<Usuario>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Advogado> Advogados => Set<Advogado>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();
    public DbSet<TipoProcesso> TiposProcesso => Set<TipoProcesso>();
    public DbSet<Tribunal> Tribunais => Set<Tribunal>();
    public DbSet<Vara> Varas => Set<Vara>();
    public DbSet<Processo> Processos => Set<Processo>();
    public DbSet<ProcessoAdvogado> ProcessosAdvogado => Set<ProcessoAdvogado>();
    public DbSet<ProcessoCliente> ProcessosCliente => Set<ProcessoCliente>();
    public DbSet<TipoAudiencia> TiposAudiencia => Set<TipoAudiencia>();
    public DbSet<Audiencia> Audiencias => Set<Audiencia>();
    public DbSet<TipoPrazo> TiposPrazo => Set<TipoPrazo>();
    public DbSet<Prazo> Prazos => Set<Prazo>();
    public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();
    public DbSet<Documento> Documentos => Set<Documento>();
    public DbSet<Movimentacao> Movimentacoes => Set<Movimentacao>();
    public DbSet<Contrato> Contratos => Set<Contrato>();
    public DbSet<Honorario> Honorarios => Set<Honorario>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();
    public DbSet<ConfiguracaoEscritorio> ConfiguracoesEscritorio => Set<ConfiguracaoEscritorio>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Composite keys for junction tables
        builder.Entity<ProcessoAdvogado>()
            .HasKey(pa => new { pa.ProcessoId, pa.AdvogadoId });

        builder.Entity<ProcessoCliente>()
            .HasKey(pc => new { pc.ProcessoId, pc.ClienteId });

        // Processo → NumeroProcesso unique
        builder.Entity<Processo>()
            .HasIndex(p => p.NumeroProcesso)
            .IsUnique();

        // Advogado → NumeroOAB unique
        builder.Entity<Advogado>()
            .HasIndex(a => a.NumeroOAB)
            .IsUnique();

        // Usuario → one-to-one Advogado
        builder.Entity<Advogado>()
            .HasOne(a => a.Usuario)
            .WithOne(u => u.Advogado)
            .HasForeignKey<Advogado>(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Usuario → one-to-one Cliente
        builder.Entity<Cliente>()
            .HasOne(c => c.Usuario)
            .WithOne(u => u.Cliente)
            .HasForeignKey<Cliente>(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Endereco → Usuario
        builder.Entity<Endereco>()
            .HasOne(e => e.Usuario)
            .WithMany(u => u.Enderecos)
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Vara → Tribunal
        builder.Entity<Vara>()
            .HasOne(v => v.Tribunal)
            .WithMany(t => t.Varas)
            .HasForeignKey(v => v.TribunalId)
            .OnDelete(DeleteBehavior.Restrict);

        // Processo → TipoProcesso
        builder.Entity<Processo>()
            .HasOne(p => p.TipoProcesso)
            .WithMany(tp => tp.Processos)
            .HasForeignKey(p => p.TipoProcessoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Processo → Tribunal
        builder.Entity<Processo>()
            .HasOne(p => p.Tribunal)
            .WithMany(t => t.Processos)
            .HasForeignKey(p => p.TribunalId)
            .OnDelete(DeleteBehavior.Restrict);

        // Processo → Vara (optional)
        builder.Entity<Processo>()
            .HasOne(p => p.Vara)
            .WithMany(v => v.Processos)
            .HasForeignKey(p => p.VaraId)
            .OnDelete(DeleteBehavior.SetNull);

        // ProcessoAdvogado relationships
        builder.Entity<ProcessoAdvogado>()
            .HasOne(pa => pa.Processo)
            .WithMany(p => p.ProcessosAdvogado)
            .HasForeignKey(pa => pa.ProcessoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProcessoAdvogado>()
            .HasOne(pa => pa.Advogado)
            .WithMany(a => a.ProcessosAdvogado)
            .HasForeignKey(pa => pa.AdvogadoId)
            .OnDelete(DeleteBehavior.Restrict);

        // ProcessoCliente relationships
        builder.Entity<ProcessoCliente>()
            .HasOne(pc => pc.Processo)
            .WithMany(p => p.ProcessosCliente)
            .HasForeignKey(pc => pc.ProcessoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProcessoCliente>()
            .HasOne(pc => pc.Cliente)
            .WithMany(c => c.ProcessosCliente)
            .HasForeignKey(pc => pc.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Audiencia → Processo
        builder.Entity<Audiencia>()
            .HasOne(a => a.Processo)
            .WithMany(p => p.Audiencias)
            .HasForeignKey(a => a.ProcessoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audiencia → TipoAudiencia
        builder.Entity<Audiencia>()
            .HasOne(a => a.TipoAudiencia)
            .WithMany(ta => ta.Audiencias)
            .HasForeignKey(a => a.TipoAudienciaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prazo → Processo
        builder.Entity<Prazo>()
            .HasOne(pr => pr.Processo)
            .WithMany(p => p.Prazos)
            .HasForeignKey(pr => pr.ProcessoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Prazo → TipoPrazo
        builder.Entity<Prazo>()
            .HasOne(pr => pr.TipoPrazo)
            .WithMany(tp => tp.Prazos)
            .HasForeignKey(pr => pr.TipoPrazoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prazo → Advogado (optional)
        builder.Entity<Prazo>()
            .HasOne(pr => pr.Advogado)
            .WithMany(a => a.Prazos)
            .HasForeignKey(pr => pr.AdvogadoId)
            .OnDelete(DeleteBehavior.SetNull);

        // Documento → Processo
        builder.Entity<Documento>()
            .HasOne(d => d.Processo)
            .WithMany(p => p.Documentos)
            .HasForeignKey(d => d.ProcessoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Documento → TipoDocumento
        builder.Entity<Documento>()
            .HasOne(d => d.TipoDocumento)
            .WithMany(td => td.Documentos)
            .HasForeignKey(d => d.TipoDocumentoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Documento → Usuario (upload)
        builder.Entity<Documento>()
            .HasOne(d => d.UploadUsuario)
            .WithMany(u => u.DocumentosUpload)
            .HasForeignKey(d => d.UploadUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Movimentacao → Processo
        builder.Entity<Movimentacao>()
            .HasOne(m => m.Processo)
            .WithMany(p => p.Movimentacoes)
            .HasForeignKey(m => m.ProcessoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Movimentacao → Usuario
        builder.Entity<Movimentacao>()
            .HasOne(m => m.Usuario)
            .WithMany(u => u.Movimentacoes)
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Contrato → Advogado
        builder.Entity<Contrato>()
            .HasOne(c => c.Advogado)
            .WithMany(a => a.Contratos)
            .HasForeignKey(c => c.AdvogadoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Contrato → Cliente
        builder.Entity<Contrato>()
            .HasOne(c => c.Cliente)
            .WithMany(cl => cl.Contratos)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Honorario → Contrato
        builder.Entity<Honorario>()
            .HasOne(h => h.Contrato)
            .WithMany(c => c.Honorarios)
            .HasForeignKey(h => h.ContratoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notificacao → Usuario
        builder.Entity<Notificacao>()
            .HasOne(n => n.Usuario)
            .WithMany(u => u.Notificacoes)
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notificacao → Processo (optional)
        builder.Entity<Notificacao>()
            .HasOne(n => n.Processo)
            .WithMany(p => p.Notificacoes)
            .HasForeignKey(n => n.ProcessoId)
            .OnDelete(DeleteBehavior.SetNull);

        // Decimal precision
        builder.Entity<Contrato>()
            .Property(c => c.ValorHonorarios)
            .HasPrecision(18, 2);

        builder.Entity<Honorario>()
            .Property(h => h.Valor)
            .HasPrecision(18, 2);
    }
}
