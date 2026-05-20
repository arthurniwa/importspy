using InfinityImports.Core.Data;
using InfinityImports.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfinityImports.Web.Controllers;

[Authorize]
public class EncomendasController : Controller
{
	private readonly AppDbContext _context;
	
	public EncomendasController(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Index()
	{
		var encomendas = await _context.Encomendas
		.Include(e => e.Produto)
		.Include(e => e.Viagem)
		.OrderByDescending(e => e.DataEncomenda)
		.ToListAsync();

		return View(encomendas);
	}

	[HttpGet]
	public async Task<IActionResult> Create()
	{
		 ViewBag.Produtos = new SelectList(
            await _context.Produtos.Where(p => p.Ativo).OrderBy(p => p.Nome).ToListAsync(),
            "Id", "Nome");

        ViewBag.Viagens = new SelectList(
            await _context.Viagens.Where(v => v.Status == StatusViagem.Planejada).OrderBy(v => v.DataPrevista).ToListAsync(),
            "Id", "DataPrevista");

        return View();
	}
	[HttpPost]
	public async Task<IActionResult> Create(Encomenda encomenda)
	{
		encomenda.Id = 0;
		if(!ModelState.IsValid)
		{
			ViewBag.Produtos = new SelectList(await _context.Produtos.Where(p => p.Ativo).ToListAsync(), "Id", "Nome");
			ViewBag.Viagens = new SelectList(await _context.Viagens.Where(v => v.Status == StatusViagem.Planejada).ToListAsync(), "Id", "DataPrevista");

			return View(encomenda);
		}
		var produto = await _context.Produtos.FindAsync(encomenda.ProdutoId);
        if (produto != null)
            encomenda.PrecoNoMomento = produto.PrecoFinal;

        encomenda.DataEncomenda = DateTime.UtcNow;

        _context.Encomendas.Add(encomenda);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
	}

	[HttpPost]
	public async Task<IActionResult> AtualizarStatus(int id, StatusEncomenda status)
	{
		var encomenda = await _context.Encomendas.FindAsync(id);
		
		if(encomenda == null) return NotFound();

		encomenda.Status = status;
		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(Index));
	}
}