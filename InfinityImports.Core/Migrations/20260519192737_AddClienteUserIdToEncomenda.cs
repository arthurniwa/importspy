using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfinityImports.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteUserIdToEncomenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClienteUserId",
                table: "Encomendas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Encomendas_ClienteUserId",
                table: "Encomendas",
                column: "ClienteUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Encomendas_AspNetUsers_ClienteUserId",
                table: "Encomendas",
                column: "ClienteUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Encomendas_AspNetUsers_ClienteUserId",
                table: "Encomendas");

            migrationBuilder.DropIndex(
                name: "IX_Encomendas_ClienteUserId",
                table: "Encomendas");

            migrationBuilder.DropColumn(
                name: "ClienteUserId",
                table: "Encomendas");
        }
    }
}
