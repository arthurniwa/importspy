using InfinityImports.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InfinityImports.Core.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<CotacaoDolar> CotacoesDolar => Set<CotacaoDolar>();
    public DbSet<Viagem> Viagens => Set<Viagem>();
    public DbSet<Encomenda> Encomendas => Set<Encomenda>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Produto>()
            .Property(p => p.CustoUsd)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Produto>()
            .Property(p => p.Margem)
            .HasColumnType("decimal(5,4)");

        builder.Entity<Produto>()
            .Property(p => p.PrecoFinal)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Encomenda>()
            .Property(e => e.PrecoNoMomento)
            .HasColumnType("decimal(18,2)");

        builder.Entity<CotacaoDolar>()
            .Property(c => c.Valor)
            .HasColumnType("decimal(10,4)");
    }
}
