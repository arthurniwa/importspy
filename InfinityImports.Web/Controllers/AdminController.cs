using InfinityImports.Core.Data;
using InfinityImports.Core.Models;
using InfinityImports.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfinityImports.Web.Controllers;

public class AdminController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AppDbContext _context;

    public AdminController(SignInManager<ApplicationUser> signInManager, AppDbContext context)
    {
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string senha)
    {
        var result = await _signInManager.PasswordSignInAsync(email, senha, isPersistent: false, lockoutOnFailure: false);
        if (result.Succeeded) return RedirectToAction("Dashboard");

        ModelState.AddModelError("", "Email ou senha inválidos");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        ViewBag.TotalEncomendas = await _context.Encomendas
            .CountAsync(e => e.Status == StatusEncomenda.Pendente || e.Status == StatusEncomenda.Confirmada);

        ViewBag.TotalProdutos = await _context.Produtos.CountAsync(p => p.Ativo);

        ViewBag.ProximaViagem = await _context.Viagens
            .Where(v => v.Status == StatusViagem.Planejada)
            .OrderBy(v => v.DataPrevista)
            .FirstOrDefaultAsync();

        // cotação atual
        var cotacaoAtual = await _context.CotacoesDolar
            .OrderByDescending(c => c.Data)
            .FirstOrDefaultAsync();
        ViewBag.CotacaoAtual = cotacaoAtual?.Valor;

        // gráfico doughnut — encomendas por status
        var encomendas = await _context.Encomendas.ToListAsync();
        var statusGrupos = encomendas
            .GroupBy(e => e.Status)
            .Select(g => new { Label = g.Key.ToString(), Count = g.Count() })
            .ToList();

        ViewBag.StatusLabels = statusGrupos.Select(g => g.Label).ToArray();
        ViewBag.StatusValues = statusGrupos.Select(g => g.Count).ToArray();

        // gráfico de linha — cotação últimos 30 dias
        var trintaDias = DateTime.UtcNow.AddDays(-30);
        var historico = await _context.CotacoesDolar
            .Where(c => c.Data >= trintaDias)
            .OrderBy(c => c.Data)
            .ToListAsync();

        ViewBag.CotacaoLabels = historico.Select(c => c.Data.ToString("dd/MM")).ToArray();
        ViewBag.CotacaoValues = historico.Select(c => (double)c.Valor).ToArray();

        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AtualizarCotacao([FromServices] CotacaoService cotacaoService)
    {
        await cotacaoService.AtualizarCotacaoEPrecosAsync();
        return RedirectToAction("Dashboard");
    }
}
