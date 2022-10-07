using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarCrudApi.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Productos",
                type: "decimal(19,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,4)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Productos",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldUnicode: false,
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "BarId",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Pedidos",
                type: "decimal(19,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioUnitario",
                table: "PedidoDetalles",
                type: "decimal(19,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,4)");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_BarId",
                table: "Productos",
                column: "BarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Bares",
                table: "Productos",
                column: "BarId",
                principalTable: "Bares",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Bares",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_BarId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "BarId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Productos");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Productos",
                type: "decimal(19,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Productos",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldUnicode: false,
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Pedidos",
                type: "decimal(19,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioUnitario",
                table: "PedidoDetalles",
                type: "decimal(19,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,2)");

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    BarId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => new { x.ProductoId, x.BarId });
                    table.ForeignKey(
                        name: "FK_Stocks_Bares",
                        column: x => x.BarId,
                        principalTable: "Bares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stocks_Productos",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_BarId",
                table: "Stocks",
                column: "BarId");
        }
    }
}
