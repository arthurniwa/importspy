using InfinityImports.Core.Data;
using InfinityImports.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfinityImports.Web.Controllers;

public class ClienteController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AppDbContext _context;

    public ClienteController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public IActionResult Registrar() => View();

    [HttpPost]
    public async Task<IActionResult> Registrar(string nomeCompleto, string email, string telefone, string senha)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            NomeCompleto = nomeCompleto,
            PhoneNumber = telefone
        };

        var result = await _userManager.CreateAsync(user, senha);
        if (!result.Succeeded)
        {
            ViewBag.Erros = result.Errors.Select(e => e.Description).ToList();
            return View();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("MinhasEncomendas");
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string senha)
    {
        var result = await _signInManager.PasswordSignInAsync(email, senha, isPersistent: false, lockoutOnFailure: false);
        if (result.Succeeded)
            return RedirectToAction("MinhasEncomendas");

        ModelState.AddModelError("", "Email ou senha inválidos.");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> MinhasEncomendas()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login");

        var encomendas = await _context.Encomendas
            .Include(e => e.Produto)
            .Include(e => e.Viagem)
            .Where(e => e.ClienteUserId == user.Id)
            .OrderByDescending(e => e.DataEncomenda)
            .ToListAsync();

        return View(encomendas);
    }
}
