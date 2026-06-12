using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfinityImports.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddSolicitacoesEspeciaisEEncomendaNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Encomendas_Produtos_ProdutoId",
                table: "Encomendas");

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoId",
                table: "Encomendas",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "NomeProdutoSnapshot",
                table: "Encomendas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SolicitacoesEspeciais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeProduto = table.Column<string>(type: "TEXT", nullable: false),
                    LinkReferencia = table.Column<string>(type: "TEXT", nullable: true),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", nullable: true),
                    ClienteNome = table.Column<string>(type: "TEXT", nullable: false),
                    ClienteTelefone = table.Column<string>(type: "TEXT", nullable: false),
                    ClienteUserId = table.Column<string>(type: "TEXT", nullable: false),
                    PrecoOrcado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ViagemPrevistaId = table.Column<int>(type: "INTEGER", nullable: true),
                    MotivoRecusaLoja = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    DataSolicitacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataOrcamento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataResposta = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EncomendaId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacoesEspeciais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitacoesEspeciais_AspNetUsers_ClienteUserId",
                        column: x => x.ClienteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitacoesEspeciais_Encomendas_EncomendaId",
                        column: x => x.EncomendaId,
                        principalTable: "Encomendas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolicitacoesEspeciais_Viagens_ViagemPrevistaId",
                        column: x => x.ViagemPrevistaId,
                        principalTable: "Viagens",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesEspeciais_ClienteUserId",
                table: "SolicitacoesEspeciais",
                column: "ClienteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesEspeciais_EncomendaId",
                table: "SolicitacoesEspeciais",
                column: "EncomendaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesEspeciais_ViagemPrevistaId",
                table: "SolicitacoesEspeciais",
                column: "ViagemPrevistaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Encomendas_Produtos_ProdutoId",
                table: "Encomendas",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Encomendas_Produtos_ProdutoId",
                table: "Encomendas");

            migrationBuilder.DropTable(
                name: "SolicitacoesEspeciais");

            migrationBuilder.DropColumn(
                name: "NomeProdutoSnapshot",
                table: "Encomendas");

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoId",
                table: "Encomendas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Encomendas_Produtos_ProdutoId",
                table: "Encomendas",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
