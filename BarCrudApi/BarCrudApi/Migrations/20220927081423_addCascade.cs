using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarCrudApi.Migrations
{
    public partial class addCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalles_Pedidos",
                table: "PedidoDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalles_Productos",
                table: "PedidoDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Bares",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Categorias",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Bares",
                table: "Stocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Productos",
                table: "Stocks");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalles_Pedidos",
                table: "PedidoDetalles",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalles_Productos",
                table: "PedidoDetalles",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Bares",
                table: "Pedidos",
                column: "BarId",
                principalTable: "Bares",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes",
                table: "Pedidos",
                column: "ClienteDni",
                principalTable: "Personas",
                principalColumn: "Dni",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Categorias",
                table: "Productos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Bares",
                table: "Stocks",
                column: "BarId",
                principalTable: "Bares",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Productos",
                table: "Stocks",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalles_Pedidos",
                table: "PedidoDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalles_Productos",
                table: "PedidoDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Bares",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Categorias",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Bares",
                table: "Stocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Productos",
                table: "Stocks");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalles_Pedidos",
                table: "PedidoDetalles",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalles_Productos",
                table: "PedidoDetalles",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Bares",
                table: "Pedidos",
                column: "BarId",
                principalTable: "Bares",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes",
                table: "Pedidos",
                column: "ClienteDni",
                principalTable: "Personas",
                principalColumn: "Dni");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Categorias",
                table: "Productos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Bares",
                table: "Stocks",
                column: "BarId",
                principalTable: "Bares",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Productos",
                table: "Stocks",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id");
        }
    }
}
