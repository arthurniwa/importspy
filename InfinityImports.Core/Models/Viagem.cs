namespace InfinityImports.Core.Models;

public enum StatusViagem
{
    Planejada,
    EmAndamento,
    Concluida,
    Cancelada
}

public class Viagem
{
    public int Id { get; set; }
    public DateTime DataPrevista { get; set; }
    public DateTime? DataRetorno { get; set; }
    public StatusViagem Status { get; set; } = StatusViagem.Planejada;

    public decimal CustoTotalUsd { get; set; }
    public string? Observacoes { get; set; }

    public ICollection<Encomenda> Encomendas { get; set; } = [];
}
