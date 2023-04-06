using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITrivia.Migrations
{
    /// <inheritdoc />
    public partial class logic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isCorrect",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "answersComplete",
                table: "Lobbys",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isCorrect",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "answersComplete",
                table: "Lobbys");
        }
    }
}
