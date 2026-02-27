using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiProp.Migrations
{
    /// <inheritdoc />
    public partial class AddEdificioToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EdificioId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EdificioId",
                table: "AspNetUsers",
                column: "EdificioId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Edificios_EdificioId",
                table: "AspNetUsers",
                column: "EdificioId",
                principalTable: "Edificios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Edificios_EdificioId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EdificioId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EdificioId",
                table: "AspNetUsers");
        }
    }
}
