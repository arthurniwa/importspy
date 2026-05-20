using System.Reflection.Metadata.Ecma335;
using InfinityImports.Core.Data;
using InfinityImports.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfinityImports.Web.Controllers;

[Authorize]
public class CategoriasController : Controller
{
	private readonly AppDbContext _context;

	public CategoriasController(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Index()
	{
		var categorias = await _context.Categorias
		.Include(c => c.Produtos)
		.OrderBy(c => c.Nome)
		.ToListAsync();

		return View(categorias);
	}

	[HttpGet]
	public IActionResult Create() => View();

	[HttpPost]
	public async Task<IActionResult> Create(Categoria categoria)
	{
		categoria.Id = 0;
		if(!ModelState.IsValid) return View(categoria);

		_context.Categorias.Add(categoria);
		await _context.SaveChangesAsync();
		
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return NotFound();
        return View(categoria);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Categoria categoria)
    {
        if (id != categoria.Id) return BadRequest();
        if (!ModelState.IsValid) return View(categoria);

        _context.Categorias.Update(categoria);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return NotFound();

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
