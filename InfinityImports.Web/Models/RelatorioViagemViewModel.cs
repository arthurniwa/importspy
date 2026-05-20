namespace InfinityImports.Web.Models;

public class RelatorioViagemViewModel
{
	public int ViagemId { get; set; }
	public DateTime DataViagem { get; set; }
	public int TotalEncomendas { get; set; }
	public decimal TotalArrecadado { get; set; }
	public decimal CustoTotalUsd { get; set; }
	public decimal CotacaoUsada { get; set; }
	public decimal CustoTotalBrl => CustoTotalUsd * CotacaoUsada;
	public decimal LucroLiquido => TotalArrecadado - CustoTotalBrl;

	public List<ItemRelatorio> Itens { get; set; } = [];
}

public class ItemRelatorio
{
	public string ProdutoNome { get; set; } = string.Empty;
	public int Quantidade { get; set; }
	public decimal CustoUnitarioUsd { get; set; }
	public decimal PrecoVendido { get; set; }
	public decimal CotacaoUsada { get; set; }
	public decimal CustoTotalBrl => CustoUnitarioUsd * CotacaoUsada * Quantidade;
	public decimal TotalVendido => PrecoVendido * Quantidade;
	public decimal Lucro => TotalVendido - CustoTotalBrl;
}