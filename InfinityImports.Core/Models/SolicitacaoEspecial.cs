namespace InfinityImports.Core.Models;

	public enum StatusSolicitacao
	{
		Solicitada,
		Orcada,
		AprovadaPeloCliente,
		RecusadaPeloCliente,
		RecusadaPelaLoja,
		ConvertidaEmEncomenda
	}

	public class SolicitacaoEspecial
	{
		public int Id { get; set; }

		// ── Dados do produto desejado ──
		public string NomeProduto { get; set; } = string.Empty;
		public string? LinkReferencia { get; set; }
		public int Quantidade { get; set; } = 1;
		public string? Observacoes { get; set; }

		// ── Dados do cliente ──
		public string ClienteNome { get; set; } = string.Empty;
		public string ClienteTelefone { get; set; } = string.Empty;
		public string ClienteUserId { get; set; } = string.Empty;
		public ApplicationUser? ClienteUser { get; set; }

		// ── Cotação preenchida pela loja ──
		public decimal? PrecoOrcado { get; set; }
		public int? ViagemPrevistaId { get; set; }
		public Viagem? ViagemPrevista { get; set; }
		public string? MotivoRecusaLoja { get; set; }

		// ── Estado ──
		public StatusSolicitacao Status { get; set; } = StatusSolicitacao.Solicitada;
		public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
		public DateTime? DataOrcamento { get; set; }
		public DateTime? DataResposta { get; set; }

		// ── Link com encomenda criada (após aprovação) ──
		public int? EncomendaId { get; set; }
		public Encomenda? Encomenda { get; set; }
	}
