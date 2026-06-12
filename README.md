# JusticeFlow API

## Tecnologias

- **.NET 10** — ASP.NET Core Web API
- **Entity Framework Core 10** — ORM com migrations
- **SQL Server** (LocalDB / SQL Express)
- **ASP.NET Core Identity** — gerenciamento de usuários e roles
- **JWT Bearer** — autenticação e autorização stateless
- **Swagger / OpenAPI** — documentação interativa da API
- **ViaCEP** — consulta de endereço por CEP
- **BrasilAPI** — consulta de dados por CNPJ
- **CNJ Dados Abertos** — consulta de tribunais e dados jurídicos públicos

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server ou SQL Server Express instalado localmente

## Como rodar localmente

### 1. Clone o repositório

```bash
git clone <url-do-repositorio>
cd JusticeFlow_API
```

### 2. Configure a connection string

Edite o arquivo `JusticeFlow/appsettings.json` e ajuste a string de conexão conforme sua instância do SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=JusticeFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Aplique as migrations

```bash
cd JusticeFlow
dotnet ef database update
```

### 4. Execute a API

```bash
dotnet run
```

A API estará disponível em `https://localhost:7xxx` (a porta exata aparece no terminal).

### 5. Acesse o Swagger

Abra no navegador:

```
https://localhost:<porta>/swagger
```

## Autenticação

A API usa JWT. Para acessar endpoints protegidos:

1. Faça login em `POST /api/auth/login` e copie o token retornado.
2. No Swagger, clique em **Authorize** e cole o token no formato `Bearer <token>`.
