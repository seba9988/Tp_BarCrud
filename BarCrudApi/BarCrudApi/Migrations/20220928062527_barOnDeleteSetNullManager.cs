using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarCrudApi.Migrations
{
    public partial class barOnDeleteSetNullManager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bares_Manager",
                table: "Bares");

            migrationBuilder.AddForeignKey(
                name: "FK_Bares_Manager",
                table: "Bares",
                column: "ManagerDni",
                principalTable: "Personas",
                principalColumn: "Dni",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bares_Manager",
                table: "Bares");

            migrationBuilder.AddForeignKey(
                name: "FK_Bares_Manager",
                table: "Bares",
                column: "ManagerDni",
                principalTable: "Personas",
                principalColumn: "Dni");
        }
    }
}
