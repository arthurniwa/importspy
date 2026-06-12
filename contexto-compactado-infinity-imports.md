# Contexto Compactado — Projeto Infinity Imports

**Data da compactação:** 21/05/2026
**Chat original:** ~120 mensagens

---

## 1. Objetivo

Desenvolver um sistema web completo para a loja **Infinity Imports** — loja física que compra produtos no Paraguai semanalmente e vende no Brasil. O sistema cobre catálogo público com encomendas, gestão de estoque, financeiro e integração com cotação do dólar em tempo real.

---

## 2. Decisões Tomadas

- **Stack:** .NET 10 / ASP.NET Core MVC, SQLite, EF Core 10, ASP.NET Identity, Bootstrap 5.3, Chart.js — mesma stack do PocketFinance
- **Arquitetura:** Solution com dois projetos: `InfinityImports.Web` (MVC) e `InfinityImports.Core` (entidades, DbContext, serviços)
- **Precificação:** `PrecoFinal = CustoUsd × CotacaoAtual × (1 + Margem)` — margem por produto, sem taxa separada
- **Cotação:** AwesomeAPI (gratuita, brasileira) — atualização diária às 08:00 via BackgroundService
- **Alerta cambial:** E-mail via Gmail SMTP quando variação > 3%
- **Delete de produto:** Soft delete (`Ativo = false`) — nunca apaga, pois pode ter encomendas históricas
- **Conta de cliente:** Identity completo (registro + login + MinhasEncomendas) — não só consulta por telefone
- **Layout admin:** Sidebar fixa estilo PocketFinance com toggle claro/escuro (CSS variables + localStorage)
- **CotacaoBackgroundService:** Fica no projeto Web (não no Core) — depende de `Microsoft.Extensions.Hosting`
- **Cultura:** `InvariantCulture` forçado via `RequestLocalizationOptions` — resolve bug de decimal pt-BR (0.20 → 20)

---

## 3. Estado Atual

**Fases concluídas:** 1 (Planejamento), 2 (Banco), 3 (Frontend), 4 (Backend), 5 (Autenticação)

**Fase 6 (Deploy):** Pendente — hospedagem e domínio ainda não definidos

**O que está funcionando:**
- Login admin (`admin@infinityimports.com` / `admin123`)
- CRUD completo: Produtos, Categorias, Viagens, Encomendas
- Catálogo público + página de produto + formulário de encomenda
- Registro/login de cliente + MinhasEncomendas
- Cotação automática via AwesomeAPI (busca na inicialização se banco vazio)
- Alerta de e-mail para variação cambial > 3%
- Controle de estoque (EntradaEstoque ao voltar do Paraguai)
- Relatório financeiro por viagem
- Dashboard com gráfico doughnut (encomendas por status) e linha (cotação 30 dias)
- Toggle claro/escuro com persistência em localStorage
- Sidebar com Bootstrap Icons

**Banco de dados:** Limpo (dados de teste removidos), pronto para uso real

**Git:** Repositório configurado e enviado. `.gitignore` exclui `.claude/`, `bin/`, `obj/`, `*.db`, `wwwroot/img/produtos/`

---

## 4. Arquivos e Artefatos Relevantes

| Arquivo | Status | Descrição |
|---------|--------|-----------|
| `InfinityImports.Core/Models/Produto.cs` | Criado | Nome, CustoUsd, Margem, PrecoFinal, Estoque, Ativo, CategoriaId |
| `InfinityImports.Core/Models/Categoria.cs` | Criado | Id, Nome |
| `InfinityImports.Core/Models/Viagem.cs` | Criado | DataPrevista, DataRetorno, StatusViagem (enum), CustoTotalUsd |
| `InfinityImports.Core/Models/Encomenda.cs` | Criado | ClienteNome, ClienteTelefone, ClienteUserId, ProdutoId, ViagemId, StatusEncomenda (enum), PrecoNoMomento |
| `InfinityImports.Core/Models/CotacaoDolar.cs` | Criado | Data, Valor (decimal) |
| `InfinityImports.Core/Models/ApplicationUser.cs` | Criado | Extends IdentityUser, adiciona NomeCompleto |
| `InfinityImports.Core/Data/AppDbContext.cs` | Criado | IdentityDbContext<ApplicationUser>, todos os DbSets |
| `InfinityImports.Core/Services/CotacaoService.cs` | Criado | BuscarCotacaoAtualAsync(), AtualizarCotacaoEPrecosAsync() |
| `InfinityImports.Core/Services/EmailService.cs` | Criado | EnviarAlertaCambialAsync() via Gmail SMTP |
| `InfinityImports.Web/Services/CotacaoBackgroundService.cs` | Criado | BackgroundService, roda diariamente às 08:00 |
| `InfinityImports.Web/Controllers/AdminController.cs` | Criado | Login, Logout, Dashboard, AtualizarCotacao |
| `InfinityImports.Web/Controllers/ProdutosController.cs` | Criado | CRUD + Reativar (soft delete reverso) |
| `InfinityImports.Web/Controllers/CategoriasController.cs` | Criado | CRUD completo |
| `InfinityImports.Web/Controllers/ViagensController.cs` | Criado | CRUD + Detalhes + EntradaEstoque + Relatorio |
| `InfinityImports.Web/Controllers/EncomendasController.cs` | Criado | Index + Create + AtualizarStatus |
| `InfinityImports.Web/Controllers/HomeController.cs` | Criado | Catálogo público + Produto + Encomendar + Confirmacao |
| `InfinityImports.Web/Controllers/ClienteController.cs` | Criado | Registrar + Login + Logout + MinhasEncomendas |
| `InfinityImports.Web/Models/RelatorioViagemViewModel.cs` | Criado | RelatorioViagemViewModel + ItemRelatorio |
| `InfinityImports.Web/Views/Shared/_Layout.cshtml` | Criado | Sidebar admin com Bootstrap Icons, toggle tema, logout |
| `InfinityImports.Web/Views/Shared/_LayoutPublico.cshtml` | Criado | Layout área pública com navbar e botões de login/registro |
| `InfinityImports.Web/Views/Admin/Login.cshtml` | Criado | Layout null, form simples |
| `InfinityImports.Web/Views/Admin/Dashboard.cshtml` | Criado | Cards + Chart.js (doughnut + linha) |
| `InfinityImports.Web/Views/Produtos/*.cshtml` | Criado | Index, Create, Edit |
| `InfinityImports.Web/Views/Categorias/*.cshtml` | Criado | Index, Create, Edit |
| `InfinityImports.Web/Views/Viagens/*.cshtml` | Criado | Index, Create, Edit, Detalhes, EntradaEstoque, Relatorio |
| `InfinityImports.Web/Views/Encomendas/*.cshtml` | Criado | Index, Create |
| `InfinityImports.Web/Views/Home/*.cshtml` | Criado | Index, Produto, Encomendar, Confirmacao |
| `InfinityImports.Web/Views/Cliente/*.cshtml` | Criado | Registrar, Login, MinhasEncomendas |
| `InfinityImports.Web/wwwroot/css/site.css` | Criado | Design system completo com CSS variables para dark/light |
| `InfinityImports.Web/appsettings.json` | Editado | ConnectionString SQLite + configurações de e-mail |
| `InfinityImports.Web/Program.cs` | Editado | Identity, EF Core, serviços, seed, InvariantCulture |
| `.gitignore` | Criado | Exclui .claude/, bin/, obj/, *.db, wwwroot/img/produtos/ |

---

## 5. Código e Configurações Críticas

### Program.cs — configurações essenciais

```csharp
// Cultura invariante (resolve bug decimal pt-BR)
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture),
    SupportedCultures = [CultureInfo.InvariantCulture],
    SupportedUICultures = [CultureInfo.InvariantCulture]
});

// ModelState — evita falha silenciosa em navigation properties
builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

// Serviços registrados
builder.Services.AddHttpClient<CotacaoService>();
builder.Services.AddScoped<CotacaoService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddHostedService<CotacaoBackgroundService>();

// Seed: busca cotação inicial se banco vazio
if (!await db.CotacoesDolar.AnyAsync())
{
    var cotacaoService = scope.ServiceProvider.GetRequiredService<CotacaoService>();
    await cotacaoService.AtualizarCotacaoEPrecosAsync();
}
```

### Fórmula de precificação

```
PrecoFinal = CustoUsd × CotacaoAtual.Valor × (1 + Margem)
// Margem em decimal: 0.20 = 20%
```

### appsettings.json — estrutura

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=infinityimports.db"
  },
  "Email": {
    "Smtp": "smtp.gmail.com",
    "Porta": 587,
    "Remetente": "arthurniwa94@gmail.com",
    "Senha": "[SENHA DE APP DO GMAIL]",
    "Destinatario": "arthurniwa@icloud.com"
  }
}
```

### Comandos do projeto

```bash
# Rodar
cd InfinityImports.Web && dotnet run --launch-profile http

# Migration
dotnet ef migrations add NOME --project ../InfinityImports.Core --startup-project .
dotnet ef database update --project ../InfinityImports.Core --startup-project .

# Porta padrão: http://localhost:5197
# Admin: /Admin/Login  |  Público: /
# Credenciais admin: admin@infinityimports.com / admin123
```

### Identidade visual

```
Fundo escuro: #0a0a0a
Cor principal (azul): #1A3FD4
Texto: #ffffff
Sidebar: #0d0d0d
```

---

## 6. Erros e Armadilhas Conhecidas

- **`Margem=0.20` virava `20`:** Cultura pt-BR trata `.` como separador de milhar. Resolvido com `RequestLocalizationOptions` + `InvariantCulture`. Nunca remover essa config.
- **ModelState falhava silenciosamente ao criar produto/categoria:** Navigation properties não-nullable (`Categoria Categoria`, `ICollection<Encomenda>`) eram tratadas como required pelo .NET 10. Resolvido com `SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true`.
- **UNIQUE constraint failed ao criar produto:** Dados de teste deixados no banco conflitavam. Resolvido limpando o banco e adicionando `entidade.Id = 0` em todos os Create POSTs.
- **`CotacaoBackgroundService` no projeto Core:** `BackgroundService` requer `Microsoft.Extensions.Hosting` que não está disponível em class library. Mover sempre para o projeto Web.
- **`AddHttpClient<CotacaoService>()` + `AddScoped<CotacaoService>()`:** Ambos registrados — o scoped vence. Funciona, mas é redundante. Pode simplificar no futuro.
- **appsettings.json com senha real foi para o git:** Avaliar mover para variáveis de ambiente antes do deploy.

---

## 7. Próximos Passos

- [ ] **Deploy — Fase 6:** Escolher hospedagem (Railway, Azure, VPS), configurar domínio, publicar
- [ ] **Variáveis de ambiente:** Mover credenciais de e-mail do `appsettings.json` para env vars antes do deploy
- [ ] **Refatoração visual (opcional):** Substituir classes Bootstrap (`table-dark`, `form-control bg-dark`) pelas classes customizadas `ii-*` do `site.css`
- [ ] **Notificação para cliente:** Avisar cliente por WhatsApp/e-mail quando encomenda chegar (atualmente só visível em MinhasEncomendas)
- [ ] **Conversar com os donos da Infinity** para validar o sistema antes do deploy

---

## 8. Informações Pendentes

- Canal de alerta do dono para variação cambial está configurado como e-mail — WhatsApp foi descartado por enquanto mas pode ser revisitado
- Modelo de negócio do sistema (freela fixo, mensalidade ou porcentagem) ainda não definido
- Hospedagem não escolhida

---

> **Instrução para o próximo chat:** Este arquivo contém o contexto compactado do projeto Infinity Imports. Use-o como base para continuar o trabalho. Não peça ao usuário para repetir informações que já estão aqui. Comece confirmando brevemente que entendeu o contexto e pergunte por onde o usuário quer continuar.
