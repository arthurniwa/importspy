using InfinityImports.Core.Data;
using InfinityImports.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SQLitePCL;
using InfinityImports.Web.Models;

namespace InfinityImports.Web.Controllers;

[Authorize]
public class ViagensController : Controller
{
	private readonly AppDbContext _context;

	public ViagensController(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Index()
	{
		var viagens = await _context.Viagens
		.Include(v => v.Encomendas)
		.OrderByDescending(v => v.DataPrevista)
		.ToListAsync();

		return View(viagens);
	}

	[HttpGet]
	public IActionResult Create() => View();

	[HttpPost]
	public async Task<IActionResult> Create(Viagem viagem)
	{
		viagem.Id = 0;
		if(!ModelState.IsValid) return View(viagem);

		_context.Viagens.Add(viagem);
		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}
	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		var viagem = await _context.Viagens.FindAsync(id);
		if(viagem == null) return NotFound();

		return View(viagem);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(int id, Viagem viagem)
	{
		if(id != viagem.Id) return BadRequest();
		
		if(!ModelState.IsValid) return View(viagem);

		_context.Viagens.Update(viagem);
		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}
	public async Task <IActionResult> Detalhes(int id)
	{
		var viagem = await _context.Viagens
		.Include(v => v.Encomendas)
		.ThenInclude(e => e.Produto)
		.FirstOrDefaultAsync(v => v.Id == id);

		if(viagem == null) return NotFound();
		

		return View(viagem);
	}

	[HttpGet]
public async Task<IActionResult> EntradaEstoque(int id)
{
    var viagem = await _context.Viagens
        .Include(v => v.Encomendas)
            .ThenInclude(e => e.Produto)
        .FirstOrDefaultAsync(v => v.Id == id);

    if (viagem == null) return NotFound();
    return View(viagem);
}

[HttpPost]
public async Task<IActionResult> EntradaEstoque(int id, Dictionary<int, int> quantidades)
{
    var viagem = await _context.Viagens
        .Include(v => v.Encomendas)
            .ThenInclude(e => e.Produto)
        .FirstOrDefaultAsync(v => v.Id == id);

    if (viagem == null) return NotFound();

    foreach (var (produtoId, qtd) in quantidades)
    {
        var produto = await _context.Produtos.FindAsync(produtoId);
        if (produto != null && qtd > 0)
            produto.Estoque += qtd;
    }

    viagem.Status = StatusViagem.Concluida;
    viagem.DataRetorno = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return RedirectToAction("Detalhes", new { id });
}

public async Task<IActionResult> Relatorio(int id)
{
    var viagem = await _context.Viagens
        .Include(v => v.Encomendas)
            .ThenInclude(e => e.Produto)
        .FirstOrDefaultAsync(v => v.Id == id);

    if (viagem == null) return NotFound();

    var cotacao = await _context.CotacoesDolar
        .Where(c => c.Data <= (viagem.DataRetorno ?? DateTime.UtcNow))
        .OrderByDescending(c => c.Data)
        .FirstOrDefaultAsync();

    var cotacaoValor = cotacao?.Valor ?? 0;

    var itens = viagem.Encomendas
        .GroupBy(e => e.Produto)
        .Select(g => new ItemRelatorio
        {
            ProdutoNome = g.Key.Nome,
            Quantidade = g.Sum(e => e.Quantidade),
            CustoUnitarioUsd = g.Key.CustoUsd,
            PrecoVendido = g.Average(e => e.PrecoNoMomento),
            CotacaoUsada = cotacaoValor
        })
        .ToList();

    var vm = new RelatorioViagemViewModel
    {
        ViagemId = viagem.Id,
        DataViagem = viagem.DataPrevista,
        TotalEncomendas = viagem.Encomendas.Count,
        TotalArrecadado = viagem.Encomendas.Sum(e => e.PrecoNoMomento * e.Quantidade),
        CustoTotalUsd = itens.Sum(i => i.CustoUnitarioUsd * i.Quantidade),
        CotacaoUsada = cotacaoValor,
        Itens = itens
    };

    return View(vm);
}
}	