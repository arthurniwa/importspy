using InfinityImports.Core.Data;
using InfinityImports.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace InfinityImports.Web.Controllers
{
	public class SolicitacoesEspeciaisController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public SolicitacoesEspeciaisController(AppDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[Authorize]
		[HttpGet]
		public IActionResult Criar() => View();

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Criar(SolicitacaoEspecial solicitacao)
		{
			var user = await _userManager.GetUserAsync(User);

			if(user == null) return RedirectToAction("Login", "Cliente");

			solicitacao.ClienteUserId = user.Id;
			solicitacao.ClienteNome = string.IsNullOrWhiteSpace(solicitacao.ClienteNome) ? user.NomeCompleto : solicitacao.ClienteNome;
			solicitacao.ClienteTelefone = string.IsNullOrWhiteSpace(solicitacao.ClienteTelefone) ? (user.PhoneNumber ?? "") : solicitacao.ClienteTelefone;
			solicitacao.Status = StatusSolicitacao.Solicitada;
			solicitacao.DataSolicitacao = DateTime.UtcNow;
			
			_context.SolicitacoesEspeciais.Add(solicitacao);
			await _context.SaveChangesAsync();

			return RedirectToAction("Confirmacao", new { id = solicitacao.Id });

		}
		[Authorize]
		public async Task<IActionResult> Confirmacao(int id)
		{
			var s = await _context.SolicitacoesEspeciais.FindAsync(id);

			if(s == null) return NotFound();

			return View(s);
		}

		[Authorize]
		public async Task<IActionResult> MinhasSolicitacoes()
		{
			var user = await _userManager.GetUserAsync(User);

			if(user == null) return RedirectToAction("Login", "Cliente");

			var lista = await _context.SolicitacoesEspeciais
			.Include(s => s.ViagemPrevista)
			.Where(s => s.ClienteUserId == user.Id)
			.OrderByDescending(s => s.DataSolicitacao)
			.ToListAsync();

			return View(lista);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Aprovar(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			if(user == null) return RedirectToAction("Login", "Cliente");

			var s = await _context.SolicitacoesEspeciais
			.Include(x => x.ViagemPrevista)
			.FirstOrDefaultAsync(x => x.Id == id && x.ClienteUserId == user.Id);

			if(s == null || s.Status != StatusSolicitacao.Orcada) return BadRequest();

			var encomenda = new Encomenda
			{
				ClienteNome = s.ClienteNome,
				ClienteTelefone = s.ClienteTelefone,
				ClienteUserId = s.ClienteUserId,
				Quantidade = s.Quantidade,
				PrecoNoMomento = s.PrecoOrcado ?? 0,
				ViagemId = s.ViagemPrevistaId,
				ProdutoId = null,
				NomeProdutoSnapshot = s.NomeProduto,
				Status = StatusEncomenda.Confirmada,
				DataEncomenda = DateTime.UtcNow,
				Observacoes = $"[Encomenda especial] {s.Observacoes}".Trim()
			};

			_context.Encomendas.Add(encomenda);
			await _context.SaveChangesAsync();

			s.EncomendaId = encomenda.Id;
			s.Status = StatusSolicitacao.ConvertidaEmEncomenda;
			s.DataResposta = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			return RedirectToAction("MinhasEncomendas", "Cliente");
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Recusar(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			
			if(user == null) return RedirectToAction("Login", "Cliente");

			var s = await _context.SolicitacoesEspeciais
			.FirstOrDefaultAsync(x => x.Id == id && x.ClienteUserId == user.Id);

			if(s == null || s.Status != StatusSolicitacao.Orcada) return BadRequest();

			s.Status = StatusSolicitacao.RecusadaPeloCliente;
			s.DataResposta = DateTime.UtcNow;
			await _context.SaveChangesAsync();


			return RedirectToAction(nameof(MinhasSolicitacoes));
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Admin()
		{
			var lista = await _context.SolicitacoesEspeciais
				.Include(s => s.ViagemPrevista)
				.Include(s => s.ClienteUser)
				.OrderByDescending(s => s.DataSolicitacao)
				.ToListAsync();
			return View(lista);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> Orcar(int id)
		{
			var s = await _context.SolicitacoesEspeciais.FindAsync(id);

			if(s == null) return NotFound();

			ViewBag.Viagens = await _context.Viagens
			.Where(v => v.Status == StatusViagem.Planejada)
			.OrderBy(v => v.DataPrevista)
			.ToListAsync();


			return View(s);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Orcar(int id, decimal precoOrcado, int viagemPrevistaId)
		{
			var s = await _context.SolicitacoesEspeciais.FindAsync(id);
			if (s == null) return NotFound();

			s.PrecoOrcado = precoOrcado;
			s.ViagemPrevistaId = viagemPrevistaId;
			s.Status = StatusSolicitacao.Orcada;
			s.DataOrcamento = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Admin));
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> RecusarPelaLoja(int id, string motivo)
		{
			var s = await _context.SolicitacoesEspeciais.FindAsync(id);
			if (s == null) return NotFound();

			s.Status = StatusSolicitacao.RecusadaPelaLoja;
			s.MotivoRecusaLoja = motivo;
			s.DataResposta = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Admin));
		}
	}
}