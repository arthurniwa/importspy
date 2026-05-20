using InfinityImports.Core.Data;
using InfinityImports.Core.Models;
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
		var result = await
		_signInManager.PasswordSignInAsync(email, senha, isPersistent : false, lockoutOnFailure: false);

		if(result.Succeeded) return RedirectToAction("Dashboard");
		
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
			.CountAsync( e => e.Status == StatusEncomenda.Pendente || 
			e.Status == StatusEncomenda.Confirmada);

			ViewBag.TotalProdutos = await _context.Produtos.CountAsync(p => p.Ativo);

			ViewBag.ProximaViagem = await _context.Viagens
			.Where( v => v.Status == StatusViagem.Planejada)
			.OrderBy(v => v.DataPrevista)
			.FirstOrDefaultAsync();

			return View();
		}
}