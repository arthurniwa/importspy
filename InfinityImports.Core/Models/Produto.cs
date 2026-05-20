namespace InfinityImports.Core.Models;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Foto { get; set; }

    public decimal CustoUsd { get; set; }
    public decimal Margem { get; set; }      // percentual, ex: 0.20 para 20%
    public decimal PrecoFinal { get; set; }  // calculado: CustoUsd × Cotacao × (1 + Margem)

    public int Estoque { get; set; }
    public bool Ativo { get; set; } = true;

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public ICollection<Encomenda> Encomendas { get; set; } = [];
}
