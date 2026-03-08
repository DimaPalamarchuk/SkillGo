using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillGo.Migrations
{
    /// <inheritdoc />
    public partial class AddChatNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastReadByUserAAtUtc",
                table: "Conversations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReadByUserBAtUtc",
                table: "Conversations",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReadByUserAAtUtc",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "LastReadByUserBAtUtc",
                table: "Conversations");
        }
    }
}
