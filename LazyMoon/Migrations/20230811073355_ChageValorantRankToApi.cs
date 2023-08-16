using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LazyMoon.Migrations
{
    public partial class ChageValorantRankToApi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_TTS_TTSId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_ValorantRanks_ValorantRankId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "currentRank",
                table: "ValorantRanks");

            migrationBuilder.DropColumn(
                name: "currentScore",
                table: "ValorantRanks");

            migrationBuilder.RenameColumn(
                name: "LoginId",
                table: "Users",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Voices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NickName",
                table: "ValorantRanks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "ValorantRanks",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ValorantRankId",
                table: "Users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TTSId",
                table: "Users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_TTS_TTSId",
                table: "Users",
                column: "TTSId",
                principalTable: "TTS",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ValorantRanks_ValorantRankId",
                table: "Users",
                column: "ValorantRankId",
                principalTable: "ValorantRanks",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_TTS_TTSId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_ValorantRanks_ValorantRankId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Voices");

            migrationBuilder.DropColumn(
                name: "NickName",
                table: "ValorantRanks");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "ValorantRanks");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "LoginId");

            migrationBuilder.AddColumn<int>(
                name: "currentRank",
                table: "ValorantRanks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "currentScore",
                table: "ValorantRanks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ValorantRankId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TTSId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_TTS_TTSId",
                table: "Users",
                column: "TTSId",
                principalTable: "TTS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ValorantRanks_ValorantRankId",
                table: "Users",
                column: "ValorantRankId",
                principalTable: "ValorantRanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
