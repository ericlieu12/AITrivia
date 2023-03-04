using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITrivia.Migrations
{
    /// <inheritdoc />
    public partial class addedBackwardsRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TriviaQuestion_Lobbys_LobbyId",
                table: "TriviaQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_TriviaQuestion_TriviaAnswer_correctAnswerId",
                table: "TriviaQuestion");

            migrationBuilder.DropIndex(
                name: "IX_TriviaQuestion_correctAnswerId",
                table: "TriviaQuestion");

            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsLeader",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "correctAnswerId",
                table: "TriviaQuestion");

            migrationBuilder.AlterColumn<int>(
                name: "LobbyId",
                table: "TriviaQuestion",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TriviaQuestionId",
                table: "TriviaAnswer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TriviaAnswer_TriviaQuestionId",
                table: "TriviaAnswer",
                column: "TriviaQuestionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TriviaAnswer_TriviaQuestion_TriviaQuestionId",
                table: "TriviaAnswer",
                column: "TriviaQuestionId",
                principalTable: "TriviaQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TriviaQuestion_Lobbys_LobbyId",
                table: "TriviaQuestion",
                column: "LobbyId",
                principalTable: "Lobbys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TriviaAnswer_TriviaQuestion_TriviaQuestionId",
                table: "TriviaAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_TriviaQuestion_Lobbys_LobbyId",
                table: "TriviaQuestion");

            migrationBuilder.DropIndex(
                name: "IX_TriviaAnswer_TriviaQuestionId",
                table: "TriviaAnswer");

            migrationBuilder.DropColumn(
                name: "TriviaQuestionId",
                table: "TriviaAnswer");

            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLeader",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "LobbyId",
                table: "TriviaQuestion",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "correctAnswerId",
                table: "TriviaQuestion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TriviaQuestion_correctAnswerId",
                table: "TriviaQuestion",
                column: "correctAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TriviaQuestion_Lobbys_LobbyId",
                table: "TriviaQuestion",
                column: "LobbyId",
                principalTable: "Lobbys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TriviaQuestion_TriviaAnswer_correctAnswerId",
                table: "TriviaQuestion",
                column: "correctAnswerId",
                principalTable: "TriviaAnswer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
