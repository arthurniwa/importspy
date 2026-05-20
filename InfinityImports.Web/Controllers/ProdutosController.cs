using InfinityImports.Core.Data;
using InfinityImports.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InfinityImports.Web.Controllers;

[Authorize]
public class ProdutosController : Controller
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var produtos = await _context.Produtos
            .Include(p => p.Categoria)
            .OrderBy(p => p.Categoria.Nome)
            .ThenBy(p => p.Nome)
            .ToListAsync();

        return View(produtos);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "Id", "Nome");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Produto produto, IFormFile? foto)
    {
        produto.Id = 0;

        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "Id", "Nome");
            return View(produto);
        }

        if (foto != null && foto.Length > 0)
        {
            var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(foto.FileName)}";
            var caminho = Path.Combine("wwwroot", "img", "produtos", nomeArquivo);
            Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);
            using var stream = System.IO.File.Create(caminho);
            await foto.CopyToAsync(stream);
            produto.Foto = $"/img/produtos/{nomeArquivo}";
        }

        var cotacao = await _context.CotacoesDolar
            .OrderByDescending(c => c.Data)
            .FirstOrDefaultAsync();

        if (cotacao != null)
            produto.PrecoFinal = produto.CustoUsd * cotacao.Valor * (1 + produto.Margem);

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null) return NotFound();

        ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "Id", "Nome", produto.CategoriaId);
        return View(produto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Produto produto, IFormFile? foto)
    {
        if (id != produto.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "Id", "Nome");
            return View(produto);
        }

        var produtoExistente = await _context.Produtos.FindAsync(id);
        if (produtoExistente == null) return NotFound();

        if (foto != null && foto.Length > 0)
        {
            var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(foto.FileName)}";
            var caminho = Path.Combine("wwwroot", "img", "produtos", nomeArquivo);
            Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);
            using var stream = System.IO.File.Create(caminho);
            await foto.CopyToAsync(stream);
            produtoExistente.Foto = $"/img/produtos/{nomeArquivo}";
        }

        var cotacao = await _context.CotacoesDolar
            .OrderByDescending(c => c.Data)
            .FirstOrDefaultAsync();

        produtoExistente.Nome = produto.Nome;
        produtoExistente.CustoUsd = produto.CustoUsd;
        produtoExistente.Margem = produto.Margem;
        produtoExistente.CategoriaId = produto.CategoriaId;
        produtoExistente.Ativo = produto.Ativo;

        if (cotacao != null)
            produtoExistente.PrecoFinal = produtoExistente.CustoUsd * cotacao.Valor * (1 + produtoExistente.Margem);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null) return NotFound();

        produto.Ativo = false;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Reativar(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null) return NotFound();

        produto.Ativo = true;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
