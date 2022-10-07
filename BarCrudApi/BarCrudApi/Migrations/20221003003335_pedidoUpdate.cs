using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarCrudApi.Migrations
{
    public partial class pedidoUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Bares",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Pedidos");

            migrationBuilder.AlterColumn<int>(
                name: "BarId",
                table: "Pedidos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCompra",
                table: "Pedidos",
                type: "date",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Bares_BarId",
                table: "Pedidos",
                column: "BarId",
                principalTable: "Bares",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Bares_BarId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "FechaCompra",
                table: "Pedidos");

            migrationBuilder.AlterColumn<int>(
                name: "BarId",
                table: "Pedidos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Pedidos",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Bares",
                table: "Pedidos",
                column: "BarId",
                principalTable: "Bares",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
