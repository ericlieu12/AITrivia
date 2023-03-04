using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITrivia.Migrations
{
    /// <inheritdoc />
    public partial class addedTriviaQA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "usersDone",
                table: "Lobbys");

            migrationBuilder.AddColumn<int>(
                name: "TriviaQuestionId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isDone",
                table: "Lobbys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TriviaAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    answerString = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaAnswer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TriviaQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    questionString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    correctAnswerId = table.Column<int>(type: "int", nullable: false),
                    LobbyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TriviaQuestion_Lobbys_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "Lobbys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TriviaQuestion_TriviaAnswer_correctAnswerId",
                        column: x => x.correctAnswerId,
                        principalTable: "TriviaAnswer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TriviaQuestionId",
                table: "Users",
                column: "TriviaQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TriviaQuestion_correctAnswerId",
                table: "TriviaQuestion",
                column: "correctAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_TriviaQuestion_LobbyId",
                table: "TriviaQuestion",
                column: "LobbyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_TriviaQuestion_TriviaQuestionId",
                table: "Users",
                column: "TriviaQuestionId",
                principalTable: "TriviaQuestion",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_TriviaQuestion_TriviaQuestionId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "TriviaQuestion");

            migrationBuilder.DropTable(
                name: "TriviaAnswer");

            migrationBuilder.DropIndex(
                name: "IX_Users_TriviaQuestionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TriviaQuestionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "isDone",
                table: "Lobbys");

            migrationBuilder.AddColumn<int>(
                name: "usersDone",
                table: "Lobbys",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
