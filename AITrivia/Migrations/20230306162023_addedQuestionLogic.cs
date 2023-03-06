using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITrivia.Migrations
{
    /// <inheritdoc />
    public partial class addedQuestionLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_TriviaQuestion_TriviaQuestionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TriviaQuestionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TriviaAnswer_TriviaQuestionId",
                table: "TriviaAnswer");

            migrationBuilder.DropColumn(
                name: "TriviaQuestionId",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "isCorrect",
                table: "TriviaAnswer",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "questionNumber",
                table: "Lobbys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TriviaAnswer_TriviaQuestionId",
                table: "TriviaAnswer",
                column: "TriviaQuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TriviaAnswer_TriviaQuestionId",
                table: "TriviaAnswer");

            migrationBuilder.DropColumn(
                name: "isCorrect",
                table: "TriviaAnswer");

            migrationBuilder.DropColumn(
                name: "questionNumber",
                table: "Lobbys");

            migrationBuilder.AddColumn<int>(
                name: "TriviaQuestionId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TriviaQuestionId",
                table: "Users",
                column: "TriviaQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TriviaAnswer_TriviaQuestionId",
                table: "TriviaAnswer",
                column: "TriviaQuestionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_TriviaQuestion_TriviaQuestionId",
                table: "Users",
                column: "TriviaQuestionId",
                principalTable: "TriviaQuestion",
                principalColumn: "Id");
        }
    }
}
