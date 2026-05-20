using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InfinityImports.Core.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task EnviarAlertaCambialAsync(decimal cotacaoAnterior, decimal cotacaoAtual, decimal variacao)
    {
        try
        {
            var smtp = _config["Email:Smtp"]!;
            var porta = int.Parse(_config["Email:Porta"]!);
            var remetente = _config["Email:Remetente"]!;
            var senha = _config["Email:Senha"]!;
            var destinatario = _config["Email:Destinatario"]!;

            var mensagem = new MailMessage
            {
                From = new MailAddress(remetente, "Infinity Imports"),
                Subject = $"⚠️ Dólar variou {variacao:F2}% — Infinity Imports",
                Body = $"""
                    Olá!

                    A cotação do dólar teve uma variação significativa hoje.

                    Cotação anterior: R$ {cotacaoAnterior:F4}
                    Cotação atual:    R$ {cotacaoAtual:F4}
                    Variação:         {variacao:F2}%

                    Os preços dos produtos já foram atualizados automaticamente.

                    — Sistema Infinity Imports
                    """,
                IsBodyHtml = false
            };

            mensagem.To.Add(destinatario);

            using var client = new SmtpClient(smtp, porta)
            {
                Credentials = new NetworkCredential(remetente, senha),
                EnableSsl = true
            };

            await client.SendMailAsync(mensagem);
            _logger.LogInformation("Alerta cambial enviado para {Destinatario}", destinatario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar alerta cambial.");
        }
    }
}
