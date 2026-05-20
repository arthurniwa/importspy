using InfinityImports.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfinityImports.Web.Services;

public class CotacaoBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CotacaoBackgroundService> _logger;

    public CotacaoBackgroundService(IServiceScopeFactory scopeFactory, ILogger<CotacaoBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var agora = DateTime.Now;
            var proximaExecucao = DateTime.Today.AddDays(1).AddHours(8);
            var espera = proximaExecucao - agora;

            if (espera < TimeSpan.Zero)
                espera = TimeSpan.FromMinutes(1);

            _logger.LogInformation("Próxima atualização de cotação em {Espera}", espera);
            await Task.Delay(espera, stoppingToken);

            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<CotacaoService>();
            var variacao = await service.AtualizarCotacaoEPrecosAsync();

            _logger.LogInformation("Cotação atualizada. Variação: {Variacao:F2}%", variacao);
        }
    }
}
