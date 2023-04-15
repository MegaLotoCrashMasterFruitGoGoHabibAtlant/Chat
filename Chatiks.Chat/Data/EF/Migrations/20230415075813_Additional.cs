using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chatiks.Chat.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class Additional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "ChatMessages",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<long>(
                name: "RepliedMessageId",
                table: "ChatMessages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_RepliedMessageId",
                table: "ChatMessages",
                column: "RepliedMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatMessages_RepliedMessageId",
                table: "ChatMessages",
                column: "RepliedMessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatMessages_RepliedMessageId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_RepliedMessageId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "RepliedMessageId",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "ChatMessages",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
