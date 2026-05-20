namespace InfinityImports.Core.Models;

public enum StatusEncomenda
{
    Pendente,
    Confirmada,
    EmViagem,
    Chegou,
    Entregue,
    Cancelada
}

public class Encomenda
{
    public int Id { get; set; }

    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteTelefone { get; set; } = string.Empty;

    public int Quantidade { get; set; } = 1;
    public decimal PrecoNoMomento { get; set; }  // snapshot do preço no ato da encomenda
    public StatusEncomenda Status { get; set; } = StatusEncomenda.Pendente;

    public DateTime DataEncomenda { get; set; } = DateTime.UtcNow;
    public string? Observacoes { get; set; }

    public int ProdutoId { get; set; }
    public Produto Produto { get; set; } = null!;

    public int? ViagemId { get; set; }
    public Viagem? Viagem { get; set; }

    public string? ClienteUserId { get; set; }
    public ApplicationUser? ClienteUser { get; set; }
}
