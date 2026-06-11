using JusticeFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JusticeFlow.Data;

public static class SeedData
{
    public static async Task InicializarAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        var context     = services.GetRequiredService<AppDbContext>();

        await CriarRolesAsync(roleManager);
        await CriarUsuariosAsync(userManager, context);
        await CriarDadosDominioAsync(context);
        await ExecutarSeedSqlAsync(context);
    }

    // ----------------------------------------------------------------
    // seed.sql: copiado para o output pelo .csproj (CopyToOutputDirectory=Always)
    // ----------------------------------------------------------------
    public static async Task ExecutarSeedSqlAsync(AppDbContext context)
    {
        var sqlPath = Path.Combine(AppContext.BaseDirectory, "seed.sql");
        if (!File.Exists(sqlPath)) return;

        var sql  = await File.ReadAllTextAsync(sqlPath);
        var conn = context.Database.GetDbConnection();

        bool opened = conn.State != System.Data.ConnectionState.Open;
        if (opened) await conn.OpenAsync();
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText    = sql;
            cmd.CommandTimeout = 120;
            await cmd.ExecuteNonQueryAsync();
        }
        finally
        {
            if (opened) conn.Close();
        }
    }

    // ----------------------------------------------------------------
    // Roles
    // ----------------------------------------------------------------
    private static async Task CriarRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = ["Administrador", "Advogado", "Cliente"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // ----------------------------------------------------------------
    // Usuários + perfis
    // ----------------------------------------------------------------
    private static async Task CriarUsuariosAsync(UserManager<Usuario> userManager, AppDbContext context)
    {
        var usuariosSeed = new[]
        {
            new { Email = "admin@justiceflow.com",    Nome = "Administrador JusticeFlow", Senha = "Admin@123",    Role = "Administrador", CPF = (string?)null              },
            new { Email = "adv1@justiceflow.com",     Nome = "Dr. Carlos Mendes",         Senha = "Advogado@123", Role = "Advogado",      CPF = (string?)"111.222.333-44"  },
            new { Email = "adv2@justiceflow.com",     Nome = "Dra. Ana Souza",            Senha = "Advogado@123", Role = "Advogado",      CPF = (string?)"222.333.444-55"  },
            new { Email = "adv3@justiceflow.com",     Nome = "Dr. Ricardo Lima",          Senha = "Advogado@123", Role = "Advogado",      CPF = (string?)"555.111.222-33"  },
            new { Email = "cliente1@justiceflow.com", Nome = "João Silva",                Senha = "Cliente@123",  Role = "Cliente",       CPF = (string?)"333.444.555-66"  },
            new { Email = "cliente2@justiceflow.com", Nome = "Maria Oliveira",            Senha = "Cliente@123",  Role = "Cliente",       CPF = (string?)"444.555.666-77"  },
            new { Email = "cliente3@justiceflow.com", Nome = "Pedro Costa",               Senha = "Cliente@123",  Role = "Cliente",       CPF = (string?)"555.666.777-88"  },
            new { Email = "cliente4@justiceflow.com", Nome = "ABC Consultoria Ltda",      Senha = "Cliente@123",  Role = "Cliente",       CPF = (string?)null              },
            new { Email = "cliente5@justiceflow.com", Nome = "Sandra Ferreira",           Senha = "Cliente@123",  Role = "Cliente",       CPF = (string?)"666.777.888-99"  },
        };

        foreach (var seed in usuariosSeed)
        {
            if (await userManager.FindByEmailAsync(seed.Email) != null) continue;

            var usuario = new Usuario
            {
                UserName       = seed.Email,
                Email          = seed.Email,
                NomeCompleto   = seed.Nome,
                CPF            = seed.CPF,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(usuario, seed.Senha);
            if (!result.Succeeded) continue;

            await userManager.AddToRoleAsync(usuario, seed.Role);

            if (seed.Role == "Advogado")
            {
                var (oab, uf, esp) = seed.Email switch
                {
                    var e when e.Contains("adv1") => ("SP123456", "SP", "Direito Civil"),
                    var e when e.Contains("adv2") => ("RJ654321", "RJ", "Direito Trabalhista"),
                    _                             => ("MG789012", "MG", "Direito Penal")
                };
                context.Advogados.Add(new Advogado
                {
                    UsuarioId     = usuario.Id,
                    NumeroOAB     = oab,
                    UF            = uf,
                    Especialidade = esp,
                    DataAdmissao  = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3)),
                    Status        = StatusAdvogado.Ativo
                });
            }
            else if (seed.Role == "Cliente")
            {
                var cli = new Cliente
                {
                    UsuarioId    = usuario.Id,
                    Tipo         = seed.Email.Contains("cliente4") ? TipoCliente.PessoaJuridica : TipoCliente.PessoaFisica,
                    DataCadastro = DateTime.UtcNow
                };
                if (seed.Email.Contains("cliente4"))
                {
                    cli.CNPJ        = "12.345.678/0001-90";
                    cli.RazaoSocial = "ABC Consultoria Ltda";
                    cli.NomeFantasia = "ABC Consultoria";
                }
                context.Clientes.Add(cli);
            }
        }

        await context.SaveChangesAsync();
    }

    // ----------------------------------------------------------------
    // Dados de domínio (tipos, tribunais, configuração)
    // ----------------------------------------------------------------
    private static async Task CriarDadosDominioAsync(AppDbContext context)
    {
        if (!context.TiposProcesso.Any())
        {
            context.TiposProcesso.AddRange(
                new TipoProcesso { Nome = "Cível",          Descricao = "Disputas entre particulares" },
                new TipoProcesso { Nome = "Criminal",       Descricao = "Infrações penais" },
                new TipoProcesso { Nome = "Trabalhista",    Descricao = "Relações de trabalho" },
                new TipoProcesso { Nome = "Família",        Descricao = "Divórcio, guarda, inventário" },
                new TipoProcesso { Nome = "Tributário",     Descricao = "Questões fiscais e tributárias" },
                new TipoProcesso { Nome = "Previdenciário", Descricao = "Benefícios do INSS" },
                new TipoProcesso { Nome = "Administrativo", Descricao = "Atos da administração pública" }
            );
        }

        if (!context.TiposAudiencia.Any())
        {
            context.TiposAudiencia.AddRange(
                new TipoAudiencia { Nome = "Instrução",   Descricao = "Coleta de provas e depoimentos" },
                new TipoAudiencia { Nome = "Julgamento",  Descricao = "Decisão final do juiz" },
                new TipoAudiencia { Nome = "Conciliação", Descricao = "Tentativa de acordo entre as partes" },
                new TipoAudiencia { Nome = "Mediação",    Descricao = "Mediação extrajudicial" },
                new TipoAudiencia { Nome = "Inaugural",   Descricao = "Primeira audiência do processo" },
                new TipoAudiencia { Nome = "Una",         Descricao = "Audiência una — instrução e julgamento reunidos" }
            );
        }

        if (!context.TiposPrazo.Any())
        {
            context.TiposPrazo.AddRange(
                new TipoPrazo { Nome = "Contestação",  DiasDefault = 15, Descricao = "Prazo para contestar a ação" },
                new TipoPrazo { Nome = "Recursal",     DiasDefault = 15, Descricao = "Prazo para interposição de recurso" },
                new TipoPrazo { Nome = "Manifestação", DiasDefault = 5,  Descricao = "Manifestação sobre documentos" },
                new TipoPrazo { Nome = "Impugnação",   DiasDefault = 10, Descricao = "Impugnar valor da causa ou execução" },
                new TipoPrazo { Nome = "Embargos",     DiasDefault = 5,  Descricao = "Embargos de declaração" },
                new TipoPrazo { Nome = "Diligência",   DiasDefault = 30, Descricao = "Cumprimento de diligência" }
            );
        }

        if (!context.TiposDocumento.Any())
        {
            context.TiposDocumento.AddRange(
                new TipoDocumento { Nome = "Petição Inicial", Descricao = "Documento que inicia o processo" },
                new TipoDocumento { Nome = "Contestação",     Descricao = "Defesa do réu" },
                new TipoDocumento { Nome = "Recurso",         Descricao = "Impugnação de decisão" },
                new TipoDocumento { Nome = "Procuração",      Descricao = "Autorização para representação" },
                new TipoDocumento { Nome = "Contrato",        Descricao = "Instrumento contratual" },
                new TipoDocumento { Nome = "Sentença",        Descricao = "Decisão judicial de mérito" },
                new TipoDocumento { Nome = "Acórdão",         Descricao = "Decisão colegiada de tribunal" },
                new TipoDocumento { Nome = "Laudo Pericial",  Descricao = "Relatório de perito judicial" },
                new TipoDocumento { Nome = "Documento Geral", Descricao = "Outros documentos" }
            );
        }

        if (!context.Tribunais.Any())
        {
            context.Tribunais.AddRange(
                new Tribunal { Nome = "Supremo Tribunal Federal",                Sigla = "STF",  Tipo = TipoTribunal.Superior,    Estado = null },
                new Tribunal { Nome = "Superior Tribunal de Justiça",            Sigla = "STJ",  Tipo = TipoTribunal.Superior,    Estado = null },
                new Tribunal { Nome = "Tribunal Superior do Trabalho",           Sigla = "TST",  Tipo = TipoTribunal.Trabalhista, Estado = null },
                new Tribunal { Nome = "Tribunal de Justiça de São Paulo",        Sigla = "TJSP", Tipo = TipoTribunal.Estadual,    Estado = "SP" },
                new Tribunal { Nome = "Tribunal de Justiça do Rio de Janeiro",   Sigla = "TJRJ", Tipo = TipoTribunal.Estadual,    Estado = "RJ" },
                new Tribunal { Nome = "Tribunal de Justiça de Minas Gerais",     Sigla = "TJMG", Tipo = TipoTribunal.Estadual,    Estado = "MG" },
                new Tribunal { Nome = "Tribunal Regional do Trabalho 2ª Região", Sigla = "TRT2", Tipo = TipoTribunal.Trabalhista, Estado = "SP" },
                new Tribunal { Nome = "Tribunal Regional Federal 3ª Região",     Sigla = "TRF3", Tipo = TipoTribunal.Federal,     Estado = "SP" }
            );
        }

        if (!context.ConfiguracoesEscritorio.Any())
        {
            context.ConfiguracoesEscritorio.Add(new ConfiguracaoEscritorio
            {
                NomeEscritorio = "JusticeFlow Advocacia",
                CNPJ           = "00.000.000/0001-00",
                Email          = "contato@justiceflow.com",
                Telefone       = "(11) 99999-9999"
            });
        }

        await context.SaveChangesAsync();
    }
}
