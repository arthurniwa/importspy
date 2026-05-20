using System.Text.Json;
using InfinityImports.Core.Data;
using InfinityImports.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfinityImports.Core.Services;

public class CotacaoService
{
	private readonly AppDbContext _context;
	private readonly HttpClient _httpClient;
	private readonly ILogger<CotacaoService> _logger;
	private readonly EmailService _emailService;

	public CotacaoService(AppDbContext context, HttpClient httpClient, ILogger<CotacaoService> logger, EmailService emailService)
	{
		_context = context;
		_httpClient = httpClient;
		_logger = logger;
		_emailService = emailService;
	}

	public async Task<decimal?> BuscarCotacaoAtualAsync()
	{
		try
		{
			var response = await
			_httpClient.GetStringAsync("https://economia.awesomeapi.com.br/json/last/USD-BRL");
			var json = JsonDocument.Parse(response);
			var bid = json.RootElement.GetProperty("USDBRL").GetProperty("bid").GetString();

			return decimal.Parse(bid!, System.Globalization.CultureInfo.InvariantCulture);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Erro ao buscar cotação do dólar");

			return null;
		}
	}
	public async Task<decimal> AtualizarCotacaoEPrecosAsync()
	{
		var cotacaoAtual = await BuscarCotacaoAtualAsync();

		if(cotacaoAtual == null) return 0;

		var cotacaoAnterior = await _context.CotacoesDolar
		.OrderByDescending(c => c.Data)
		.FirstOrDefaultAsync();

		_context.CotacoesDolar.Add(new CotacaoDolar
        {
            Data = DateTime.UtcNow,
            Valor = cotacaoAtual.Value
        });

		var produtos = await _context.Produtos.Where(p => p.Ativo).ToListAsync();

		foreach(var produto in produtos)
		produto.PrecoFinal = produto.CustoUsd * cotacaoAtual.Value * (1 + produto.Margem);

		await _context.SaveChangesAsync();

		if (cotacaoAnterior == null) return 0;

		var variacao = Math.Abs((cotacaoAtual.Value - cotacaoAnterior.Valor) / cotacaoAnterior.Valor * 100);

		if (variacao >= 3)
			await _emailService.EnviarAlertaCambialAsync(cotacaoAnterior.Valor, cotacaoAtual.Value, variacao);

		return variacao;
	}
}