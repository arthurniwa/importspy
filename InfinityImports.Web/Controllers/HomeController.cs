using System.Diagnostics;
using InfinityImports.Core.Data;
using InfinityImports.Core.Models;
using InfinityImports.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfinityImports.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int? categoriaId)
    {
        var categorias = await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();

        var query = _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.Ativo);

        if (categoriaId.HasValue)
            query = query.Where(p => p.CategoriaId == categoriaId.Value);

        var produtos = await query.OrderBy(p => p.Nome).ToListAsync();

        ViewBag.Categorias = categorias;
        ViewBag.CategoriaAtual = categoriaId;

        return View(produtos);
    }

    public async Task<IActionResult> Produto(int id)
    {
        var produto = await _context.Produtos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativo);

        if (produto == null) return NotFound();

        var proximaViagem = await _context.Viagens
            .Where(v => v.Status == StatusViagem.Planejada)
            .OrderBy(v => v.DataPrevista)
            .FirstOrDefaultAsync();

        ViewBag.ProximaViagem = proximaViagem;
        return View(produto);
    }

    public async Task<IActionResult> Encomendar(int produtoId)
    {
        var produto = await _context.Produtos.FindAsync(produtoId);
        if (produto == null) return NotFound();

        var proximaViagem = await _context.Viagens
            .Where(v => v.Status == StatusViagem.Planejada)
            .OrderBy(v => v.DataPrevista)
            .FirstOrDefaultAsync();

        ViewBag.Produto = produto;
        ViewBag.ProximaViagem = proximaViagem;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Encomendar(Encomenda encomenda)
    {
        var produto = await _context.Produtos.FindAsync(encomenda.ProdutoId);
        if (produto == null) return NotFound();

        encomenda.PrecoNoMomento = produto.PrecoFinal;
        encomenda.DataEncomenda = DateTime.UtcNow;
        encomenda.Status = StatusEncomenda.Pendente;

        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            encomenda.ClienteUserId = user?.Id;
        }

        _context.Encomendas.Add(encomenda);
        await _context.SaveChangesAsync();

        return RedirectToAction("Confirmacao", new { id = encomenda.Id });
    }

    public async Task<IActionResult> Confirmacao(int id)
    {
        var encomenda = await _context.Encomendas
            .Include(e => e.Produto)
            .Include(e => e.Viagem)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (encomenda == null) return NotFound();
        return View(encomenda);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
