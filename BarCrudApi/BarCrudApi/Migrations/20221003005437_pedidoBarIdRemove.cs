using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarCrudApi.Migrations
{
    public partial class pedidoBarIdRemove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Bares_BarId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_BarId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "BarId",
                table: "Pedidos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BarId",
                table: "Pedidos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_BarId",
                table: "Pedidos",
                column: "BarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Bares_BarId",
                table: "Pedidos",
                column: "BarId",
                principalTable: "Bares",
                principalColumn: "Id");
        }
    }
}
