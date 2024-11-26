using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nvovka.CommandManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CommandItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CommandReferenceItems_CommandItemId",
                table: "CommandReferenceItems",
                column: "CommandItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommandReferenceItems_CommandItems_CommandItemId",
                table: "CommandReferenceItems",
                column: "CommandItemId",
                principalTable: "CommandItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommandReferenceItems_CommandItems_CommandItemId",
                table: "CommandReferenceItems");

            migrationBuilder.DropIndex(
                name: "IX_CommandReferenceItems_CommandItemId",
                table: "CommandReferenceItems");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CommandItems");
        }
    }
}
