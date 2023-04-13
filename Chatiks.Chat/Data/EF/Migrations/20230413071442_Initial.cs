using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Chatiks.Chat.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatBase",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatBase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrivateChat",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    OtherUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateChat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateChat_ChatBase_Id",
                        column: x => x.Id,
                        principalTable: "ChatBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublicChat",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ChatName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicChat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicChat_ChatBase_Id",
                        column: x => x.Id,
                        principalTable: "ChatBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalUserId = table.Column<long>(type: "bigint", nullable: false),
                    InviterId = table.Column<long>(type: "bigint", nullable: true),
                    PrivateChatId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatUsers_ChatUsers_InviterId",
                        column: x => x.InviterId,
                        principalTable: "ChatUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatUsers_PrivateChat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "PrivateChat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatUsers_PrivateChat_PrivateChatId",
                        column: x => x.PrivateChatId,
                        principalTable: "PrivateChat",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatUsers_PublicChat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "PublicChat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    ChatUserId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    EditTime = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SendTime = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatBase_ChatId",
                        column: x => x.ChatId,
                        principalTable: "ChatBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatUsers_ChatUserId",
                        column: x => x.ChatUserId,
                        principalTable: "ChatUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageLinks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatMessageId = table.Column<long>(type: "bigint", nullable: false),
                    ExternalImageId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageLinks_ChatMessages_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatBase_CreatorId",
                table: "ChatBase",
                column: "CreatorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatId",
                table: "ChatMessages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatUserId",
                table: "ChatMessages",
                column: "ChatUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_ChatId",
                table: "ChatUsers",
                column: "ChatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_InviterId",
                table: "ChatUsers",
                column: "InviterId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_PrivateChatId",
                table: "ChatUsers",
                column: "PrivateChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageLinks_ChatMessageId",
                table: "ImageLinks",
                column: "ChatMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatBase_ChatUsers_CreatorId",
                table: "ChatBase",
                column: "CreatorId",
                principalTable: "ChatUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatBase_ChatUsers_CreatorId",
                table: "ChatBase");

            migrationBuilder.DropTable(
                name: "ImageLinks");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatUsers");

            migrationBuilder.DropTable(
                name: "PrivateChat");

            migrationBuilder.DropTable(
                name: "PublicChat");

            migrationBuilder.DropTable(
                name: "ChatBase");
        }
    }
}
