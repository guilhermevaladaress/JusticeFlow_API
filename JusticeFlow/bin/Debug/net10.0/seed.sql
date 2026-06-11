-- ================================================================
-- JusticeFlow – seed.sql  (copiado para output pelo .csproj)
-- Executado automaticamente no startup após SeedData.InicializarAsync
-- Data de referência: 2026-06-10 (hoje)
--
-- KPIs resultantes no dashboard:
--   ProcessosAtivos=10 | ProcessosEncerrados=3 | AudienciasHoje=3
--   AudienciasMes=6    | PrazosVencendo=4       | PrazosCumpridos=8
--   TotalDocumentos=20 | TotalClientes=5        | TotalAdvogados=3
--   HonariosAtrasados=3| NovosClientesMes=5
-- ================================================================
SET NOCOUNT ON;

-- ================================================================
-- IDs de lookup (populados pelo SeedData.cs – InicializarAsync)
-- ================================================================
DECLARE @UsrAdv1  NVARCHAR(450); DECLARE @UsrAdv2  NVARCHAR(450); DECLARE @UsrAdv3  NVARCHAR(450);
DECLARE @UsrCli1  NVARCHAR(450); DECLARE @UsrCli2  NVARCHAR(450); DECLARE @UsrCli3  NVARCHAR(450);
DECLARE @UsrCli4  NVARCHAR(450); DECLARE @UsrCli5  NVARCHAR(450); DECLARE @UsrAdmin NVARCHAR(450);
DECLARE @Adv1Id INT; DECLARE @Adv2Id INT; DECLARE @Adv3Id INT;
DECLARE @Cli1Id INT; DECLARE @Cli2Id INT; DECLARE @Cli3Id INT; DECLARE @Cli4Id INT; DECLARE @Cli5Id INT;
DECLARE @TribTJSP INT; DECLARE @TribTJRJ INT; DECLARE @TribTRT2 INT; DECLARE @TribTJMG INT;
DECLARE @TipoCivel INT; DECLARE @TipoCrim  INT; DECLARE @TipoTrab   INT;
DECLARE @TipoFam   INT; DECLARE @TipoTrib  INT; DECLARE @TipoPrev   INT; DECLARE @TipoAdm INT;
DECLARE @TipoConc INT; DECLARE @TipoInstr INT; DECLARE @TipoInau INT; DECLARE @TipoJulg INT; DECLARE @TipoMed INT;
DECLARE @PrazoContest INT; DECLARE @PrazoRecursal INT; DECLARE @PrazoManifest INT;
DECLARE @PrazoEmbarcos INT; DECLARE @PrazoDilig INT; DECLARE @PrazoImpug INT;
DECLARE @DocPeticao INT; DECLARE @DocContest INT; DECLARE @DocProc INT; DECLARE @DocSentenca INT;
DECLARE @DocContrato INT; DECLARE @DocGeral INT; DECLARE @DocAcordao INT;
DECLARE @VaraCivelSP INT; DECLARE @VaraFamSP INT; DECLARE @VaraCrimSP INT; DECLARE @VaraTribSP INT;
DECLARE @VaraCivelRJ INT; DECLARE @VaraCrimRJ INT;
DECLARE @VaraCivelMG INT; DECLARE @VaraFamMG INT; DECLARE @VaraTrabSP INT;
DECLARE @P1 INT; DECLARE @P2 INT; DECLARE @P3  INT; DECLARE @P4  INT; DECLARE @P5  INT;
DECLARE @P6 INT; DECLARE @P7 INT; DECLARE @P8  INT; DECLARE @P9  INT; DECLARE @P10 INT;
DECLARE @P11 INT; DECLARE @P12 INT; DECLARE @P13 INT; DECLARE @P14 INT;
DECLARE @C1 INT; DECLARE @C2 INT; DECLARE @C3 INT; DECLARE @C4 INT;
DECLARE @C5 INT; DECLARE @C6 INT; DECLARE @C7 INT; DECLARE @C8 INT;

SELECT @UsrAdv1  = Id FROM AspNetUsers WHERE Email = N'adv1@justiceflow.com';
SELECT @UsrAdv2  = Id FROM AspNetUsers WHERE Email = N'adv2@justiceflow.com';
SELECT @UsrAdv3  = Id FROM AspNetUsers WHERE Email = N'adv3@justiceflow.com';
SELECT @UsrCli1  = Id FROM AspNetUsers WHERE Email = N'cliente1@justiceflow.com';
SELECT @UsrCli2  = Id FROM AspNetUsers WHERE Email = N'cliente2@justiceflow.com';
SELECT @UsrCli3  = Id FROM AspNetUsers WHERE Email = N'cliente3@justiceflow.com';
SELECT @UsrCli4  = Id FROM AspNetUsers WHERE Email = N'cliente4@justiceflow.com';
SELECT @UsrCli5  = Id FROM AspNetUsers WHERE Email = N'cliente5@justiceflow.com';
SELECT @UsrAdmin = Id FROM AspNetUsers WHERE Email = N'admin@justiceflow.com';
SELECT @Adv1Id = Id FROM Advogados WHERE NumeroOAB = N'SP123456';
SELECT @Adv2Id = Id FROM Advogados WHERE NumeroOAB = N'RJ654321';
SELECT @Adv3Id = Id FROM Advogados WHERE NumeroOAB = N'MG789012';
SELECT @Cli1Id = Id FROM Clientes WHERE UsuarioId = @UsrCli1;
SELECT @Cli2Id = Id FROM Clientes WHERE UsuarioId = @UsrCli2;
SELECT @Cli3Id = Id FROM Clientes WHERE UsuarioId = @UsrCli3;
SELECT @Cli4Id = Id FROM Clientes WHERE UsuarioId = @UsrCli4;
SELECT @Cli5Id = Id FROM Clientes WHERE UsuarioId = @UsrCli5;
SELECT @TribTJSP = Id FROM Tribunais WHERE Sigla = N'TJSP';
SELECT @TribTJRJ = Id FROM Tribunais WHERE Sigla = N'TJRJ';
SELECT @TribTRT2 = Id FROM Tribunais WHERE Sigla = N'TRT2';
SELECT @TribTJMG = Id FROM Tribunais WHERE Sigla = N'TJMG';
SELECT @TipoCivel = Id FROM TiposProcesso WHERE Nome = N'Cível';
SELECT @TipoCrim  = Id FROM TiposProcesso WHERE Nome = N'Criminal';
SELECT @TipoTrab  = Id FROM TiposProcesso WHERE Nome = N'Trabalhista';
SELECT @TipoFam   = Id FROM TiposProcesso WHERE Nome = N'Família';
SELECT @TipoTrib  = Id FROM TiposProcesso WHERE Nome = N'Tributário';
SELECT @TipoPrev  = Id FROM TiposProcesso WHERE Nome = N'Previdenciário';
SELECT @TipoAdm   = Id FROM TiposProcesso WHERE Nome = N'Administrativo';
SELECT @TipoConc  = Id FROM TiposAudiencia WHERE Nome = N'Conciliação';
SELECT @TipoInstr = Id FROM TiposAudiencia WHERE Nome = N'Instrução';
SELECT @TipoInau  = Id FROM TiposAudiencia WHERE Nome = N'Inaugural';
SELECT @TipoJulg  = Id FROM TiposAudiencia WHERE Nome = N'Julgamento';
SELECT @TipoMed   = Id FROM TiposAudiencia WHERE Nome = N'Mediação';
SELECT @PrazoContest  = Id FROM TiposPrazo WHERE Nome = N'Contestação';
SELECT @PrazoRecursal = Id FROM TiposPrazo WHERE Nome = N'Recursal';
SELECT @PrazoManifest = Id FROM TiposPrazo WHERE Nome = N'Manifestação';
SELECT @PrazoEmbarcos = Id FROM TiposPrazo WHERE Nome = N'Embargos';
SELECT @PrazoDilig    = Id FROM TiposPrazo WHERE Nome = N'Diligência';
SELECT @PrazoImpug    = Id FROM TiposPrazo WHERE Nome = N'Impugnação';
SELECT @DocPeticao  = Id FROM TiposDocumento WHERE Nome = N'Petição Inicial';
SELECT @DocContest  = Id FROM TiposDocumento WHERE Nome = N'Contestação';
SELECT @DocProc     = Id FROM TiposDocumento WHERE Nome = N'Procuração';
SELECT @DocSentenca = Id FROM TiposDocumento WHERE Nome = N'Sentença';
SELECT @DocContrato = Id FROM TiposDocumento WHERE Nome = N'Contrato';
SELECT @DocGeral    = Id FROM TiposDocumento WHERE Nome = N'Documento Geral';
SELECT @DocAcordao  = Id FROM TiposDocumento WHERE Nome = N'Acórdão';

-- ================================================================
-- 1. VARAS  (9 no total)
-- ================================================================
IF NOT EXISTS (SELECT 1 FROM Varas)
BEGIN
    INSERT INTO Varas (TribunalId, Nome, Numero, Competencia) VALUES
    (@TribTJSP, N'1ª Vara Cível Central',            1, N'Cível'),
    (@TribTJSP, N'2ª Vara de Família e Sucessões',   2, N'Família'),
    (@TribTJSP, N'3ª Vara Criminal',                 3, N'Criminal'),
    (@TribTJSP, N'4ª Vara de Fazenda Pública',       4, N'Tributário'),
    (@TribTJRJ, N'1ª Vara Cível',                    1, N'Cível'),
    (@TribTJRJ, N'1ª Vara Criminal',                 1, N'Criminal'),
    (@TribTJMG, N'1ª Vara Cível',                    1, N'Cível'),
    (@TribTJMG, N'1ª Vara de Família',               1, N'Família'),
    (@TribTRT2, N'1ª Vara do Trabalho de SP',         1, N'Trabalhista');
END;

SELECT TOP 1 @VaraCivelSP = Id FROM Varas WHERE TribunalId = @TribTJSP AND Competencia = N'Cível';
SELECT TOP 1 @VaraFamSP   = Id FROM Varas WHERE TribunalId = @TribTJSP AND Competencia = N'Família';
SELECT TOP 1 @VaraCrimSP  = Id FROM Varas WHERE TribunalId = @TribTJSP AND Competencia = N'Criminal';
SELECT TOP 1 @VaraTribSP  = Id FROM Varas WHERE TribunalId = @TribTJSP AND Competencia = N'Tributário';
SELECT TOP 1 @VaraCivelRJ = Id FROM Varas WHERE TribunalId = @TribTJRJ AND Competencia = N'Cível';
SELECT TOP 1 @VaraCrimRJ  = Id FROM Varas WHERE TribunalId = @TribTJRJ AND Competencia = N'Criminal';
SELECT TOP 1 @VaraCivelMG = Id FROM Varas WHERE TribunalId = @TribTJMG AND Competencia = N'Cível';
SELECT TOP 1 @VaraFamMG   = Id FROM Varas WHERE TribunalId = @TribTJMG AND Competencia = N'Família';
SELECT TOP 1 @VaraTrabSP  = Id FROM Varas WHERE TribunalId = @TribTRT2;

-- ================================================================
-- 2. ENDEREÇOS  (1 por usuário não-admin)
-- ================================================================
IF NOT EXISTS (SELECT 1 FROM Enderecos)
BEGIN
    INSERT INTO Enderecos (UsuarioId, CEP, Logradouro, Numero, Complemento, Bairro, Cidade, Estado, Principal) VALUES
    (@UsrAdv1,  N'01310-100', N'Avenida Paulista',           N'1000', N'Sala 501',   N'Bela Vista',     N'São Paulo',      N'SP', 1),
    (@UsrAdv2,  N'20040-020', N'Rua do Ouvidor',             N'50',   NULL,          N'Centro',          N'Rio de Janeiro', N'RJ', 1),
    (@UsrAdv3,  N'30130-100', N'Avenida Afonso Pena',        N'2500', N'Sala 310',   N'Centro',          N'Belo Horizonte', N'MG', 1),
    (@UsrCli1,  N'01415-000', N'Rua Augusta',                N'300',  N'Apto 42',   N'Consolação',      N'São Paulo',      N'SP', 1),
    (@UsrCli2,  N'04538-133', N'Av. Brigadeiro Faria Lima',  N'2000', NULL,          N'Itaim Bibi',      N'São Paulo',      N'SP', 1),
    (@UsrCli3,  N'04001-001', N'Rua Vergueiro',              N'800',  N'Apto 15',   N'Paraíso',         N'São Paulo',      N'SP', 1),
    (@UsrCli4,  N'04533-082', N'Av. Faria Lima',             N'3500', N'8º Andar',  N'Itaim Bibi',      N'São Paulo',      N'SP', 1),
    (@UsrCli5,  N'80250-210', N'Rua XV de Novembro',         N'722',  NULL,          N'Centro',          N'Curitiba',       N'PR', 1),
    (@UsrAdmin, N'01310-930', N'Avenida Paulista',            N'900',  N'10º Andar', N'Bela Vista',      N'São Paulo',      N'SP', 1);
END;

-- ================================================================
-- 3. PROCESSOS  (14 no total)
--   Ativo(0)=10  Suspenso(1)=1  Encerrado(2)=3
-- ================================================================
IF NOT EXISTS (SELECT 1 FROM Processos)
BEGIN
    INSERT INTO Processos (NumeroProcesso, Titulo, Descricao, Status, DataAbertura, DataEncerramento, TipoProcessoId, TribunalId, VaraId) VALUES
    -- 1 - Ativo
    (N'0001234-55.2024.8.26.0100', N'Indenização por Danos Morais',
     N'Negativação indevida em cadastros de restrição ao crédito. Cliente comprova prejuízo à honra e imagem.',
     0, '2024-03-15', NULL, @TipoCivel, @TribTJSP, @VaraCivelSP),
    -- 2 - Ativo
    (N'0009876-11.2024.8.26.0100', N'Divórcio Consensual c/ Partilha de Bens',
     N'Partilha de imóvel avaliado em R$ 450.000 e guarda compartilhada de filhos menores.',
     0, '2024-06-20', NULL, @TipoFam, @TribTJSP, @VaraFamSP),
    -- 3 - Ativo
    (N'0005432-78.2024.5.02.0001', N'Reclamação Trabalhista – Verbas Rescisórias',
     N'Horas extras não pagas (3h/dia, 5 anos), FGTS e indenização por dispensa sem justa causa.',
     0, '2024-08-10', NULL, @TipoTrab, @TribTRT2, @VaraTrabSP),
    -- 4 - Encerrado
    (N'0002468-33.2023.8.19.0001', N'Cobrança de Alugueis em Atraso',
     N'12 meses de alugueis em atraso (R$ 2.400/mês). Encerrado com êxito após pagamento integral.',
     2, '2023-01-10', '2024-01-10', @TipoCivel, @TribTJRJ, @VaraCivelRJ),
    -- 5 - Ativo [audiência HOJE + prazo Jun/12]
    (N'0003579-44.2025.8.26.0100', N'Fraude em Contrato de Prestação de Serviços',
     N'Empresa descumpriu contrato de TI causando prejuízo de R$ 120.000. Pedido de rescisão e ressarcimento.',
     0, '2025-02-10', NULL, @TipoCivel, @TribTJSP, @VaraCivelSP),
    -- 6 - Ativo [prazo Jun/14]
    (N'0004680-55.2025.8.26.0100', N'Indenização por Acidente de Trânsito',
     N'Colisão traseira em rodovia. Lesões corporais e danos materiais ao veículo do autor.',
     0, '2025-04-05', NULL, @TipoCivel, @TribTJSP, @VaraCivelSP),
    -- 7 - Ativo [audiência HOJE]
    (N'0005791-66.2025.8.26.0100', N'Revisão de Pensão Alimentícia',
     N'Pedido de revisão do valor fixado há 3 anos devido à mudança na capacidade financeira do alimentante.',
     0, '2025-05-20', NULL, @TipoFam, @TribTJSP, @VaraFamSP),
    -- 8 - Ativo [audiência HOJE + prazo Jun/11]
    (N'0006802-77.2025.8.26.0100', N'Crime de Estelionato – Venda Fraudulenta',
     N'Suspeito vendeu imóvel com documentação falsa a múltiplas vítimas. Prejuízo total estimado em R$ 800.000.',
     0, '2025-06-01', NULL, @TipoCrim, @TribTJSP, @VaraCrimSP),
    -- 9 - Suspenso
    (N'0007913-88.2025.8.19.0001', N'Rescisão de Contrato Imobiliário',
     N'Incorporadora não entregou imóvel no prazo. Processo suspenso por tentativa de acordo extrajudicial.',
     1, '2025-03-15', NULL, @TipoCivel, @TribTJRJ, @VaraCivelRJ),
    -- 10 - Ativo [prazo Jun/16 + audiência Jun/15]
    (N'0008024-99.2025.8.26.0100', N'Revisão de Benefício INSS – Aposentadoria',
     N'Cliente aposentado por invalidez com benefício calculado incorretamente. Pedido de revisão e diferenças.',
     0, '2025-07-12', NULL, @TipoPrev, @TribTJSP, @VaraCivelSP),
    -- 11 - Ativo [audiência Jun/25]
    (N'0009135-01.2025.8.26.0100', N'Ação Popular – Licitação Irregular',
     N'Irregularidades em licitação municipal com superfaturamento de R$ 2.000.000 em obra pública.',
     0, '2025-09-30', NULL, @TipoAdm, @TribTJSP, NULL),
    -- 12 - Encerrado
    (N'0010246-12.2022.8.13.0100', N'Reconhecimento e Dissolução de União Estável',
     N'União de 8 anos. Reconhecimento judicial e partilha de bens adquiridos em comum.',
     2, '2022-08-01', '2023-06-15', @TipoFam, @TribTJMG, @VaraFamMG),
    -- 13 - Ativo [audiência Jun/20]
    (N'0011357-23.2025.8.26.0100', N'Recuperação de Crédito Empresarial',
     N'ABC Consultoria com crédito de R$ 350.000 em aberto contra fornecedor que entrou em recuperação judicial.',
     0, '2025-10-05', NULL, @TipoTrib, @TribTJSP, @VaraTribSP),
    -- 14 - Encerrado
    (N'0012468-34.2024.8.26.0100', N'Violência Doméstica – Medida Protetiva',
     N'Vítima obteve medida protetiva de urgência. Ação penal encerrada com condenação do agressor.',
     2, '2024-01-20', '2024-11-30', @TipoCrim, @TribTJSP, @VaraCrimSP);
END;

SELECT @P1  = Id FROM Processos WHERE NumeroProcesso = N'0001234-55.2024.8.26.0100';
SELECT @P2  = Id FROM Processos WHERE NumeroProcesso = N'0009876-11.2024.8.26.0100';
SELECT @P3  = Id FROM Processos WHERE NumeroProcesso = N'0005432-78.2024.5.02.0001';
SELECT @P4  = Id FROM Processos WHERE NumeroProcesso = N'0002468-33.2023.8.19.0001';
SELECT @P5  = Id FROM Processos WHERE NumeroProcesso = N'0003579-44.2025.8.26.0100';
SELECT @P6  = Id FROM Processos WHERE NumeroProcesso = N'0004680-55.2025.8.26.0100';
SELECT @P7  = Id FROM Processos WHERE NumeroProcesso = N'0005791-66.2025.8.26.0100';
SELECT @P8  = Id FROM Processos WHERE NumeroProcesso = N'0006802-77.2025.8.26.0100';
SELECT @P9  = Id FROM Processos WHERE NumeroProcesso = N'0007913-88.2025.8.19.0001';
SELECT @P10 = Id FROM Processos WHERE NumeroProcesso = N'0008024-99.2025.8.26.0100';
SELECT @P11 = Id FROM Processos WHERE NumeroProcesso = N'0009135-01.2025.8.26.0100';
SELECT @P12 = Id FROM Processos WHERE NumeroProcesso = N'0010246-12.2022.8.13.0100';
SELECT @P13 = Id FROM Processos WHERE NumeroProcesso = N'0011357-23.2025.8.26.0100';
SELECT @P14 = Id FROM Processos WHERE NumeroProcesso = N'0012468-34.2024.8.26.0100';

-- ================================================================
-- 4. VÍNCULOS PROCESSO ↔ ADVOGADO
-- ================================================================
IF NOT EXISTS (SELECT 1 FROM ProcessosAdvogado)
BEGIN
    INSERT INTO ProcessosAdvogado (ProcessoId, AdvogadoId, DataVinculo, TipoVinculo, Ativo) VALUES
    (@P1,  @Adv1Id, '2024-03-15 10:00:00', 0, 1),
    (@P2,  @Adv1Id, '2024-06-20 09:00:00', 0, 1),
    (@P3,  @Adv2Id, '2024-08-10 08:00:00', 0, 1),
    (@P4,  @Adv2Id, '2023-01-10 09:00:00', 0, 1),
    (@P5,  @Adv1Id, '2025-02-10 10:00:00', 0, 1),
    (@P6,  @Adv1Id, '2025-04-05 09:00:00', 0, 1),
    (@P7,  @Adv2Id, '2025-05-20 08:00:00', 0, 1),
    (@P8,  @Adv3Id, '2025-06-01 09:00:00', 0, 1),
    (@P9,  @Adv3Id, '2025-03-15 10:00:00', 0, 1),
    (@P10, @Adv2Id, '2025-07-12 09:00:00', 0, 1),
    (@P11, @Adv3Id, '2025-09-30 09:00:00', 0, 1),
    (@P12, @Adv1Id, '2022-08-01 09:00:00', 0, 1),
    (@P13, @Adv2Id, '2025-10-05 10:00:00', 0, 1),
    (@P14, @Adv3Id, '2024-01-20 09:00:00', 0, 1);
END;

-- ================================================================
-- 5. VÍNCULOS PROCESSO ↔ CLIENTE
-- ================================================================
IF NOT EXISTS (SELECT 1 FROM ProcessosCliente)
BEGIN
    INSERT INTO ProcessosCliente (ProcessoId, ClienteId, DataVinculo, TipoParte) VALUES
    (@P1,  @Cli1Id, '2024-03-15 10:00:00', 0),
    (@P2,  @Cli1Id, '2024-06-20 09:00:00', 0),
    (@P3,  @Cli2Id, '2024-08-10 08:00:00', 0),
    (@P4,  @Cli2Id, '2023-01-10 09:00:00', 0),
    (@P5,  @Cli3Id, '2025-02-10 10:00:00', 0),
    (@P6,  @Cli3Id, '2025-04-05 09:00:00', 0),
    (@P7,  @Cli4Id, '2025-05-20 08:00:00', 0),
    (@P8,  @Cli2Id, '2025-06-01 09:00:00', 0),
    (@P9,  @Cli4Id, '2025-03-15 10:00:00', 0),
    (@P10, @Cli3Id, '2025-07-12 09:00:00', 0),
    (@P11, @Cli1Id, '2025-09-30 09:00:00', 0),
    (@P12, @Cli4Id, '2022-08-01 09:00:00', 0),
    (@P13, @Cli4Id, '2025-10-05 10:00:00', 0),
    (@P14, @Cli5Id, '2024-01-20 09:00:00', 0);
END;

-- ================================================================
-- 6. AUDIÊNCIAS  (18 total)
--   3 HOJE (AudienciasHoje=3)  |  6 em Junho/2026 (AudienciasMes=6)
-- ================================================================
IF NOT EXISTS (SELECT 1 FROM Audiencias)
BEGIN
    INSERT INTO Audiencias (ProcessoId, TipoAudienciaId, DataHora, Local, LinkVirtual, Status, Observacoes) VALUES
    -- Passadas / Realizadas -----------------------------------------------
    (@P1,  @TipoConc,  '2024-05-20 09:00:00', N'1ª Vara Cível Central – Sala 3',         NULL, 1, N'Partes não chegaram a acordo. Processo segue para instrução.'),
    (@P2,  @TipoMed,   '2024-09-10 10:30:00', N'2ª Vara de Família – Sala de Mediação',  NULL, 1, N'Acordo parcial sobre guarda compartilhada alcançado.'),
    (@P3,  @TipoInau,  '2024-10-05 08:00:00', N'1ª Vara do Trabalho de SP',              NULL, 1, N'Proposta de R$ 8.000 recusada pelo reclamante.'),
    (@P4,  @TipoConc,  '2023-04-12 09:00:00', N'1ª Vara Cível – RJ',                     NULL, 1, N'Réu não compareceu. Audiência redesignada.'),
    (@P4,  @TipoConc,  '2023-07-15 10:00:00', N'1ª Vara Cível – RJ',                     NULL, 1, N'Acordo de pagamento em 3 parcelas firmado.'),
    (@P5,  @TipoInstr, '2025-06-18 14:00:00', N'1ª Vara Cível Central – Sala 3',         NULL, 1, N'Oitiva de peritos sobre laudo de TI juntado.'),
    (@P7,  @TipoConc,  '2025-11-10 09:00:00', N'2ª Vara de Família – Sala de Mediação',  NULL, 1, N'Partes não chegaram a acordo sobre valor da pensão.'),
    (@P12, @TipoInstr, '2022-11-20 14:00:00', N'1ª Vara de Família – BH',                NULL, 1, N'Depoimentos das partes e testemunhas colhidos.'),
    (@P14, @TipoInau,  '2024-03-10 09:00:00', N'3ª Vara Criminal – SP',                  NULL, 1, N'Audiência inaugural com leitura da denúncia.'),
    -- HOJE 2026-06-10 (AudienciasHoje=3) ----------------------------------
    (@P5,  @TipoConc,  '2026-06-10 09:00:00', N'1ª Vara Cível Central – Sala 3',         NULL, 0, N'Segunda tentativa de conciliação com mediador nomeado.'),
    (@P7,  @TipoInstr, '2026-06-10 14:00:00', N'2ª Vara de Família – Sala 5',            NULL, 0, N'Oitiva do alimentante e apresentação de comprovantes de renda.'),
    (@P8,  @TipoInau,  '2026-06-10 10:30:00', N'3ª Vara Criminal – SP',                  NULL, 0, N'Audiência inaugural do processo de estelionato.'),
    -- Junho/2026 (contribuem para AudienciasMes=6) ------------------------
    (@P10, @TipoInstr, '2026-06-15 09:00:00', N'1ª Vara Cível Central – Sala 2',         NULL, 0, N'Apresentação do laudo pericial sobre cálculo do INSS.'),
    (@P13, @TipoConc,  '2026-06-20 14:00:00', N'4ª Vara de Fazenda Pública – SP',        NULL, 0, N'Reunião de conciliação com representante da empresa em recuperação.'),
    (@P11, @TipoInau,  '2026-06-25 10:00:00', N'1ª Vara Cível Central – Sala 1',         NULL, 0, N'Audiência inaugural da ação popular com MP como custos legis.'),
    -- Futuras --------------------------------------------------------------
    (@P1,  @TipoInstr, '2026-08-15 14:00:00', N'1ª Vara Cível Central – Sala 3',         NULL, 0, N'Oitiva de testemunhas do autor e do réu.'),
    (@P2,  @TipoConc,  '2026-10-05 09:00:00', N'2ª Vara de Família – Sala de Mediação',  NULL, 0, N'Homologação final do acordo de partilha de bens.'),
    (@P3,  @TipoJulg,  '2026-09-22 10:00:00', N'1ª Vara do Trabalho de SP',              NULL, 0, N'Audiência de julgamento trabalhista.');
END;

-- ================================================================
-- 7. PRAZOS  (22 total)
--   Cumprido(1)=8  Vencido(2)=4  Pendente(0) dentro de 7 dias=4  Pendente futuro=6
-- ================================================================
IF NOT EXISTS (SELECT 1 FROM Prazos)
BEGIN
    INSERT INTO Prazos (ProcessoId, TipoPrazoId, AdvogadoId, Descricao, DataVencimento, DataCumprimento, Status, Observacoes) VALUES
    -- Cumpridos -----------------------------------------------------------
    (@P1,  @PrazoContest,  @Adv1Id, N'Contestar a ação de danos morais',              '2024-04-15 23:59:00', '2024-04-14 16:00:00', 1, NULL),
    (@P2,  @PrazoContest,  @Adv1Id, N'Resposta ao pedido de guarda exclusiva',         '2024-07-20 23:59:00', '2024-07-18 11:00:00', 1, NULL),
    (@P3,  @PrazoContest,  @Adv2Id, N'Contrarrazões à defesa da reclamada',            '2024-09-10 23:59:00', '2024-09-08 15:00:00', 1, NULL),
    (@P4,  @PrazoManifest, @Adv2Id, N'Manifestação sobre proposta de acordo',          '2023-03-01 23:59:00', '2023-02-28 14:00:00', 1, NULL),
    (@P5,  @PrazoManifest, @Adv1Id, N'Manifestação sobre laudo pericial de TI',        '2025-05-10 23:59:00', '2025-05-09 17:00:00', 1, NULL),
    (@P12, @PrazoContest,  @Adv1Id, N'Contestar reconhecimento da união estável',      '2022-10-01 23:59:00', '2022-09-30 10:00:00', 1, NULL),
    (@P7,  @PrazoImpug,    @Adv2Id, N'Impugnar valor apresentado pelo alimentante',    '2025-12-15 23:59:00', '2025-12-14 16:00:00', 1, NULL),
    (@P14, @PrazoManifest, @Adv3Id, N'Manifestação sobre laudos da perícia criminal',  '2024-07-20 23:59:00', '2024-07-19 11:00:00', 1, NULL),
    -- Vencidos ------------------------------------------------------------
    (@P4,  @PrazoEmbarcos, @Adv2Id, N'Embargos de declaração à sentença',              '2024-01-25 23:59:00', NULL, 2, N'Partes concordaram com a decisão. Prazo não utilizado.'),
    (@P6,  @PrazoContest,  @Adv1Id, N'Contestar ação do réu sobre acidente',           '2025-08-01 23:59:00', NULL, 2, N'Prazo expirado. Situação regularizada extrajudicialmente.'),
    (@P9,  @PrazoManifest, @Adv3Id, N'Manifestação sobre proposta da incorporadora',   '2025-10-15 23:59:00', NULL, 2, N'Negociação suspensa. Processo aguardando desbloqueio.'),
    (@P14, @PrazoRecursal, @Adv3Id, N'Recurso da sentença de 1ª instância',            '2024-09-01 23:59:00', NULL, 2, N'Condenação aceita. Recurso não necessário.'),
    -- Pendentes dentro de 7 dias (2026-06-10 a 2026-06-17) ---------------
    (@P8,  @PrazoContest,  @Adv3Id, N'Contestar a denúncia no processo de estelionato','2026-06-11 23:59:00', NULL, 0, N'URGENTE: Audiência inaugural é hoje. Contestação deve ser apresentada.'),
    (@P5,  @PrazoContest,  @Adv1Id, N'Apresentar contestação à defesa da ré',          '2026-06-12 23:59:00', NULL, 0, N'Após audiência de hoje, prazo começa a correr.'),
    (@P6,  @PrazoManifest, @Adv1Id, N'Manifestação sobre documentos juntados pelo réu','2026-06-14 23:59:00', NULL, 0, N'Réu juntou laudo do IML em 05/06/2026.'),
    (@P10, @PrazoRecursal, @Adv2Id, N'Recurso ao cálculo pericial do INSS',            '2026-06-16 23:59:00', NULL, 0, N'Laudo pericial publicado. Divergência de R$ 18.400.'),
    -- Pendentes futuros ---------------------------------------------------
    (@P1,  @PrazoManifest, @Adv1Id, N'Manifestação sobre documentos do réu',           '2026-07-10 23:59:00', NULL, 0, N'Réu juntou extratos bancários em 27/06/2026.'),
    (@P2,  @PrazoManifest, @Adv1Id, N'Manifestação sobre acordo de partilha de bens',  '2026-09-15 23:59:00', NULL, 0, NULL),
    (@P3,  @PrazoRecursal, @Adv2Id, N'Recurso ordinário trabalhista',                  '2026-08-01 23:59:00', NULL, 0, N'Aguardando publicação da sentença.'),
    (@P7,  @PrazoContest,  @Adv2Id, N'Nova contestação após revisão dos valores',      '2026-07-25 23:59:00', NULL, 0, NULL),
    (@P8,  @PrazoManifest, @Adv3Id, N'Manifestação sobre provas digitais colhidas',    '2026-08-20 23:59:00', NULL, 0, NULL),
    (@P11, @PrazoDilig,    @Adv3Id, N'Diligência: obter ata da licitação no município','2026-10-01 23:59:00', NULL, 0, NULL);
END;

-- ================================================================
-- 8. DOCUMENTOS  (20 ativos)
-- ================================================================
IF NOT EXISTS (SELECT 1 FROM Documentos)
BEGIN
    INSERT INTO Documentos (ProcessoId, TipoDocumentoId, UploadUsuarioId, Nome, Descricao, NomeArquivo, ConteudoBase64, MimeType, DataUpload, Status) VALUES
    (@P1,  @DocPeticao,  @UsrAdv1, N'Petição Inicial – Danos Morais',          N'Ação de indenização por negativação indevida',                        N'peticao_p1.pdf',          N'', N'application/pdf', '2024-03-15 10:00:00', 0),
    (@P1,  @DocProc,     @UsrAdv1, N'Procuração – João Silva',                 N'Procuração ad judicia com poderes amplos',                             N'procuracao_cli1.pdf',     N'', N'application/pdf', '2024-03-15 10:05:00', 0),
    (@P1,  @DocContest,  @UsrAdv1, N'Contestação da Parte Ré – P1',           N'Contestação apresentada dentro do prazo',                              N'contestacao_p1.pdf',      N'', N'application/pdf', '2024-04-14 14:00:00', 0),
    (@P2,  @DocPeticao,  @UsrAdv1, N'Petição Inicial – Divórcio',              N'Petição inicial do divórcio consensual',                               N'peticao_p2.pdf',          N'', N'application/pdf', '2024-06-20 09:00:00', 0),
    (@P2,  @DocProc,     @UsrAdv1, N'Procuração – Maria Oliveira',             N'Procuração outorgada por Maria Oliveira',                              N'procuracao_cli2.pdf',     N'', N'application/pdf', '2024-06-20 09:10:00', 0),
    (@P3,  @DocPeticao,  @UsrAdv2, N'Reclamação Trabalhista',                  N'Reclamação com holerites e CTPS como prova',                           N'reclamacao_p3.pdf',       N'', N'application/pdf', '2024-08-10 08:00:00', 0),
    (@P3,  @DocGeral,    @UsrAdv2, N'Carteira de Trabalho – P3',               N'CTPS com registros corrompidos pelo empregador',                       N'ctps_p3.pdf',             N'', N'application/pdf', '2024-08-10 08:30:00', 0),
    (@P4,  @DocSentenca, @UsrAdv2, N'Sentença – Cobrança de Aluguel',          N'Condenação ao pagamento integral com correção',                        N'sentenca_p4.pdf',         N'', N'application/pdf', '2023-12-20 16:00:00', 1),
    (@P5,  @DocPeticao,  @UsrAdv1, N'Petição Inicial – Fraude Contratual',    N'Ação de rescisão e ressarcimento por descumprimento',                  N'peticao_p5.pdf',          N'', N'application/pdf', '2025-02-10 10:00:00', 0),
    (@P5,  @DocContrato, @UsrAdv1, N'Contrato de Prestação de Serviços – P5', N'Contrato original descumprido pela ré',                                N'contrato_p5.pdf',         N'', N'application/pdf', '2025-02-10 10:15:00', 0),
    (@P6,  @DocPeticao,  @UsrAdv1, N'Petição Inicial – Acidente de Trânsito', N'Pedido de indenização por lesões e danos materiais',                   N'peticao_p6.pdf',          N'', N'application/pdf', '2025-04-05 09:00:00', 0),
    (@P6,  @DocGeral,    @UsrAdv1, N'Boletim de Ocorrência – P6',              N'BO policial e laudo do DETRAN sobre a colisão',                        N'bo_p6.pdf',               N'', N'application/pdf', '2025-04-05 09:30:00', 0),
    (@P7,  @DocPeticao,  @UsrAdv2, N'Petição de Revisão de Pensão',           N'Pedido de revisão com comprovantes de desemprego',                     N'peticao_p7.pdf',          N'', N'application/pdf', '2025-05-20 08:00:00', 0),
    (@P8,  @DocPeticao,  @UsrAdv3, N'Denúncia – Estelionato',                  N'Denúncia ministerial com provas documentais das transações',           N'denuncia_p8.pdf',         N'', N'application/pdf', '2025-06-01 09:00:00', 0),
    (@P9,  @DocPeticao,  @UsrAdv3, N'Petição Inicial – Rescisão Imobiliária', N'Ação contra incorporadora com contrato e comprovantes',                N'peticao_p9.pdf',          N'', N'application/pdf', '2025-03-15 10:00:00', 0),
    (@P10, @DocPeticao,  @UsrAdv2, N'Petição – Revisão INSS',                  N'Pedido de revisão de aposentadoria com cálculo atuarial',              N'peticao_p10.pdf',         N'', N'application/pdf', '2025-07-12 09:00:00', 0),
    (@P10, @DocGeral,    @UsrAdv2, N'Laudo Pericial – Cálculo INSS',           N'Laudo de perito contábil apontando diferença de R$ 18.400',            N'laudo_p10.pdf',           N'', N'application/pdf', '2026-05-20 14:00:00', 0),
    (@P11, @DocPeticao,  @UsrAdv3, N'Petição da Ação Popular',                 N'Ação popular com documentos da licitação e notas fiscais superfaturadas',N'peticao_p11.pdf',       N'', N'application/pdf', '2025-09-30 09:00:00', 0),
    (@P12, @DocAcordao,  @UsrAdv1, N'Acórdão – União Estável',                 N'Acórdão de 2ª instância reconhecendo a união e determinando partilha', N'acordao_p12.pdf',         N'', N'application/pdf', '2023-05-10 15:00:00', 0),
    (@P13, @DocPeticao,  @UsrAdv2, N'Petição – Crédito Empresarial',           N'Habilitação de crédito no processo de recuperação judicial',           N'peticao_p13.pdf',         N'', N'application/pdf', '2025-10-05 10:00:00', 0);
END;

-- ================================================================
-- 9. MOVIMENTAÇÕES  (30 no total)
-- ================================================================
IF NOT EXISTS (SELECT 1 FROM Movimentacoes)
BEGIN
    INSERT INTO Movimentacoes (ProcessoId, UsuarioId, DataHora, Descricao, Tipo) VALUES
    -- P1
    (@P1, @UsrAdv1, '2024-03-15 10:00:00', N'Processo cadastrado. Petição inicial protocolada na 1ª Vara Cível Central.',                       0),
    (@P1, @UsrAdv1, '2024-04-14 16:00:00', N'Contestação juntada pela parte ré dentro do prazo.',                                               3),
    (@P1, @UsrAdv1, '2024-05-20 09:30:00', N'Audiência de conciliação realizada. Partes não chegaram a acordo.',                                2),
    -- P2
    (@P2, @UsrAdv1, '2024-06-20 09:00:00', N'Processo de divórcio cadastrado e distribuído à 2ª Vara de Família.',                              0),
    (@P2, @UsrAdv1, '2024-07-18 11:00:00', N'Resposta ao pedido de guarda juntada dentro do prazo.',                                            3),
    (@P2, @UsrAdv1, '2024-09-10 11:00:00', N'Audiência de mediação concluída. Acordo parcial sobre guarda compartilhada.',                      2),
    -- P3
    (@P3, @UsrAdv2, '2024-08-10 08:00:00', N'Reclamação trabalhista protocolada na 1ª Vara do Trabalho.',                                       0),
    (@P3, @UsrAdv2, '2024-10-05 09:00:00', N'Audiência inaugural. Reclamada recusou proposta de R$ 8.000.',                                     2),
    -- P4
    (@P4, @UsrAdv2, '2023-01-10 09:00:00', N'Ação de cobrança de aluguel cadastrada e distribuída ao TJRJ.',                                    0),
    (@P4, @UsrAdv2, '2023-07-15 11:00:00', N'Acordo de pagamento firmado em audiência. Parcelas definidas.',                                    2),
    (@P4, @UsrAdv2, '2024-01-10 16:00:00', N'Processo encerrado. Réu efetuou pagamento integral. Sentença transitada em julgado.',              5),
    -- P5
    (@P5, @UsrAdv1, '2025-02-10 10:00:00', N'Ação por fraude contratual cadastrada. Documentos juntados.',                                      0),
    (@P5, @UsrAdv1, '2025-06-18 15:00:00', N'Audiência de instrução realizada. Peritos ouvidos.',                                               2),
    (@P5, @UsrAdv1, '2026-06-10 09:30:00', N'Audiência de conciliação realizada hoje. Partes mantêm posições.',                                 2),
    -- P6
    (@P6, @UsrAdv1, '2025-04-05 09:00:00', N'Ação de indenização por acidente de trânsito cadastrada.',                                         0),
    (@P6, @UsrAdv1, '2025-07-20 14:00:00', N'Laudo médico confirmando lesões. Juntado aos autos.',                                              1),
    -- P7
    (@P7, @UsrAdv2, '2025-05-20 08:00:00', N'Processo de revisão de pensão alimentícia distribuído à 2ª Vara de Família.',                     0),
    (@P7, @UsrAdv2, '2025-11-10 09:30:00', N'Audiência de conciliação sem acordo. Processo segue para instrução.',                             2),
    (@P7, @UsrAdv2, '2026-06-10 14:30:00', N'Audiência de instrução realizada. Comprovantes analisados.',                                       2),
    -- P8
    (@P8, @UsrAdv3, '2025-06-01 09:00:00', N'Processo criminal de estelionato registrado. Vítimas identificadas.',                              0),
    (@P8, @UsrAdv3, '2026-06-10 10:30:00', N'Audiência inaugural realizada. Réu cientificado da acusação.',                                     2),
    -- P9
    (@P9, @UsrAdv3, '2025-03-15 10:00:00', N'Ação de rescisão imobiliária distribuída ao TJRJ.',                                                0),
    (@P9, @UsrAdv3, '2025-11-20 09:00:00', N'Processo suspenso por 90 dias para tentativa de acordo extrajudicial.',                           5),
    -- P10
    (@P10,@UsrAdv2, '2025-07-12 09:00:00', N'Pedido de revisão de benefício INSS protocolado.',                                                 0),
    (@P10,@UsrAdv2, '2026-05-20 14:00:00', N'Laudo pericial recebido. Diferença de R$ 18.400 confirmada.',                                     1),
    -- P11
    (@P11,@UsrAdv3, '2025-09-30 09:00:00', N'Ação popular cadastrada com documentos da licitação superfaturada.',                               0),
    -- P12
    (@P12,@UsrAdv1, '2022-08-01 09:00:00', N'Ação de reconhecimento de união estável distribuída ao TJMG.',                                     0),
    (@P12,@UsrAdv1, '2023-06-15 14:00:00', N'Processo encerrado. Acórdão de 2ª instância reconheceu a união e determinou partilha.',           5),
    -- P13
    (@P13,@UsrAdv2, '2025-10-05 10:00:00', N'Habilitação de crédito em processo de recuperação judicial protocolada.',                          0),
    -- P14
    (@P14,@UsrAdv3, '2024-01-20 09:00:00', N'Ação de violência doméstica cadastrada. Medida protetiva concedida.',                              0),
    (@P14,@UsrAdv3, '2024-11-30 16:00:00', N'Condenação proferida. Agressor receberá pena de 2 anos em regime semiaberto.',                    5);
END;

-- ================================================================
-- 10. CONTRATOS  (8 no total)
-- ================================================================
IF NOT EXISTS (SELECT 1 FROM Contratos)
BEGIN
    INSERT INTO Contratos (AdvogadoId, ClienteId, DataAssinatura, DataValidade, ValorHonorarios, FormaPagamento, Status, ConteudoBase64, MimeType) VALUES
    (@Adv1Id, @Cli1Id, '2024-03-10', '2025-03-10', 5000.00, 0, 0, NULL, NULL),  -- C1: adv1/cli1 P1
    (@Adv1Id, @Cli1Id, '2024-06-15', '2025-06-15', 3500.00, 0, 0, NULL, NULL),  -- C2: adv1/cli1 P2
    (@Adv2Id, @Cli2Id, '2024-08-05', '2025-08-05', 4200.00, 1, 0, NULL, NULL),  -- C3: adv2/cli2 P3 (% êxito)
    (@Adv2Id, @Cli2Id, '2023-01-05', '2024-01-05', 2800.00, 0, 1, NULL, NULL),  -- C4: adv2/cli2 P4 encerrado
    (@Adv1Id, @Cli3Id, '2025-02-05', '2026-02-05', 8000.00, 0, 0, NULL, NULL),  -- C5: adv1/cli3 P5+P6
    (@Adv2Id, @Cli4Id, '2025-05-15', '2026-05-15', 6500.00, 0, 0, NULL, NULL),  -- C6: adv2/cli4 P7
    (@Adv3Id, @Cli2Id, '2025-05-28', '2026-05-28', 4800.00, 1, 0, NULL, NULL),  -- C7: adv3/cli2 P8 (% êxito)
    (@Adv3Id, @Cli4Id, '2025-03-10', '2026-03-10', 5500.00, 0, 0, NULL, NULL);  -- C8: adv3/cli4 P9
END;

SELECT @C1 = (SELECT TOP 1 Id FROM Contratos WHERE AdvogadoId=@Adv1Id AND ClienteId=@Cli1Id AND ValorHonorarios=5000.00);
SELECT @C2 = (SELECT TOP 1 Id FROM Contratos WHERE AdvogadoId=@Adv1Id AND ClienteId=@Cli1Id AND ValorHonorarios=3500.00);
SELECT @C3 = (SELECT TOP 1 Id FROM Contratos WHERE AdvogadoId=@Adv2Id AND ClienteId=@Cli2Id AND ValorHonorarios=4200.00);
SELECT @C4 = (SELECT TOP 1 Id FROM Contratos WHERE AdvogadoId=@Adv2Id AND ClienteId=@Cli2Id AND ValorHonorarios=2800.00);
SELECT @C5 = (SELECT TOP 1 Id FROM Contratos WHERE AdvogadoId=@Adv1Id AND ClienteId=@Cli3Id);
SELECT @C6 = (SELECT TOP 1 Id FROM Contratos WHERE AdvogadoId=@Adv2Id AND ClienteId=@Cli4Id);
SELECT @C7 = (SELECT TOP 1 Id FROM Contratos WHERE AdvogadoId=@Adv3Id AND ClienteId=@Cli2Id);
SELECT @C8 = (SELECT TOP 1 Id FROM Contratos WHERE AdvogadoId=@Adv3Id AND ClienteId=@Cli4Id);

-- ================================================================
-- 11. HONORÁRIOS  (22 total | Atrasado(2)=3)
-- ================================================================
IF NOT EXISTS (SELECT 1 FROM Honorarios)
BEGIN
    INSERT INTO Honorarios (ContratoId, Descricao, Valor, DataVencimento, DataPagamento, Status) VALUES
    -- C1 (3 parcelas – 2 pagas, 1 pendente)
    (@C1, N'1ª Parcela – Danos Morais (ato inicial)',     1666.67, '2024-04-10', '2024-04-08', 1),
    (@C1, N'2ª Parcela – Danos Morais (audiência)',       1666.67, '2024-07-10', '2024-07-09', 1),
    (@C1, N'3ª Parcela – Danos Morais (êxito)',           1666.66, '2026-10-10', NULL,          0),
    -- C2 (2 parcelas – 1 paga, 1 pendente)
    (@C2, N'1ª Parcela – Divórcio (ato inicial)',         1750.00, '2024-07-15', '2024-07-14', 1),
    (@C2, N'2ª Parcela – Divórcio (homologação)',         1750.00, '2026-10-15', NULL,          0),
    -- C3 (2 parcelas – 1 paga, 1 pendente)
    (@C3, N'1ª Parcela – Trabalhista (protocolo)',        2100.00, '2024-09-05', '2024-09-04', 1),
    (@C3, N'2ª Parcela – Trabalhista (êxito)',            2100.00, '2026-09-05', NULL,          0),
    -- C4 (1 parcela paga – contrato encerrado)
    (@C4, N'Honorários – Cobrança Aluguel (integral)',    2800.00, '2023-02-05', '2024-01-15', 1),
    -- C5 (3 parcelas: 1 paga, 1 ATRASADA, 1 pendente)
    (@C5, N'1ª Parcela – Fraude Contratual (protocolo)', 2666.67, '2025-03-05', '2025-03-04', 1),
    (@C5, N'2ª Parcela – Fraude Contratual (instrução)', 2666.67, '2025-10-05', NULL,          2),  -- ATRASADO
    (@C5, N'3ª Parcela – Fraude Contratual (êxito)',     2666.66, '2026-12-05', NULL,          0),
    -- C6 (3 parcelas: 1 paga, 1 ATRASADA, 1 pendente)
    (@C6, N'1ª Parcela – Pensão Alimentícia (início)',   2166.67, '2025-06-15', '2025-06-14', 1),
    (@C6, N'2ª Parcela – Pensão Alimentícia (instrução)',2166.67, '2025-12-15', NULL,          2),  -- ATRASADO
    (@C6, N'3ª Parcela – Pensão Alimentícia (êxito)',    2166.66, '2026-11-15', NULL,          0),
    -- C7 (2 parcelas: 1 ATRASADA, 1 pendente)
    (@C7, N'1ª Parcela – Estelionato (denúncia)',        2400.00, '2025-07-28', NULL,          2),  -- ATRASADO
    (@C7, N'2ª Parcela – Estelionato (condenação)',      2400.00, '2026-11-28', NULL,          0),
    -- C8 (2 parcelas: 1 paga, 1 pendente)
    (@C8, N'1ª Parcela – Rescisão Imobiliária (início)', 2750.00, '2025-04-10', '2025-04-09', 1),
    (@C8, N'2ª Parcela – Rescisão Imobiliária (êxito)', 2750.00, '2026-10-10', NULL,          0);
END;

-- ================================================================
-- 12. NOTIFICAÇÕES  (15 no total)
-- ================================================================
IF NOT EXISTS (SELECT 1 FROM Notificacoes)
BEGIN
    INSERT INTO Notificacoes (UsuarioId, ProcessoId, Titulo, Mensagem, Tipo, Lida, DataCriacao, DataLeitura) VALUES
    -- Adv1
    (@UsrAdv1, @P1,  N'Prazo se aproximando',           N'O prazo para manifestação no proc. 0001234 vence em 30 dias.',                                  0, 0, '2026-06-10 08:00:00', NULL),
    (@UsrAdv1, @P1,  N'Audiência de instrução marcada', N'Audiência de instrução agendada para 15/08/2026 às 14h na 1ª Vara Cível Central.',              1, 1, '2026-06-01 10:00:00', '2026-06-01 10:30:00'),
    (@UsrAdv1, @P5,  N'URGENTE: Audiência hoje',        N'Conciliação do proc. 0003579 marcada para HOJE às 09:00 na 1ª Vara Cível Central.',             1, 0, '2026-06-10 07:00:00', NULL),
    (@UsrAdv1, @P8,  N'URGENTE: Prazo vence amanhã',    N'Contestação no proc. 0006802 (Estelionato) vence em 11/06/2026. Prepare o documento.',         0, 0, '2026-06-10 07:30:00', NULL),
    -- Adv2
    (@UsrAdv2, @P7,  N'Audiência hoje – Pensão',        N'Audiência de instrução do proc. 0005791 às 14h. Compareça com os comprovantes de renda.',       1, 0, '2026-06-10 07:00:00', NULL),
    (@UsrAdv2, @P10, N'Prazo recursal em 6 dias',       N'Prazo para recurso do cálculo pericial do INSS vence em 16/06/2026. Revise o laudo.',           0, 0, '2026-06-10 08:00:00', NULL),
    (@UsrAdv2, @P3,  N'Audiência de julgamento',        N'Audiência de julgamento trabalhista marcada para 22/09/2026 às 10h.',                           1, 1, '2026-06-05 09:00:00', '2026-06-05 10:00:00'),
    -- Adv3
    (@UsrAdv3, @P8,  N'Audiência inaugural hoje',       N'Audiência inaugural do processo de Estelionato às 10:30. Chegue com 20 min de antecedência.',   1, 0, '2026-06-10 07:00:00', NULL),
    (@UsrAdv3, @P11, N'Audiência em 15 dias',           N'Audiência inaugural da Ação Popular em 25/06/2026. Prepare documentos da licitação.',           1, 0, '2026-06-10 08:30:00', NULL),
    -- Clientes
    (@UsrCli1, @P1,  N'Atualização do seu processo',    N'Nova movimentação no processo de Danos Morais. Audiência de instrução em agosto.',              2, 0, '2026-06-01 09:00:00', NULL),
    (@UsrCli1, @P2,  N'Acordo confirmado',              N'O acordo de guarda compartilhada foi homologado. Próxima etapa: partilha de bens.',              3, 1, '2024-09-11 08:00:00', '2024-09-12 10:00:00'),
    (@UsrCli2, @P3,  N'Audiência de julgamento próxima',N'Sua audiência de julgamento trabalhista foi agendada para 22/09/2026.',                         1, 0, '2026-06-10 08:30:00', NULL),
    (@UsrCli3, @P5,  N'Audiência hoje – sua presença',  N'Audiência de conciliação HOJE às 09:00. Sua presença pode ser solicitada pelo juiz.',           1, 0, '2026-06-10 07:00:00', NULL),
    (@UsrCli4, @P7,  N'Audiência hoje – pensão',        N'Audiência de instrução sobre revisão de pensão alimentícia HOJE às 14:00.',                     1, 0, '2026-06-10 07:00:00', NULL),
    -- Admin
    (@UsrAdmin, NULL, N'Sistema com dados completos',   N'JusticeFlow inicializado com 14 processos, 3 advogados e 5 clientes. Dashboard disponível.',    3, 1, '2026-06-10 00:00:00', '2026-06-10 08:00:00');
END;
