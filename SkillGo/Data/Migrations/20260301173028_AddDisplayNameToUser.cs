using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillGo.Migrations
{
    /// <inheritdoc />
    public partial class AddDisplayNameToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ServiceOffers_ServiceOfferId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOffers_Categories_CategoryId",
                table: "ServiceOffers");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ServiceOffers_ServiceOfferId",
                table: "Reports",
                column: "ServiceOfferId",
                principalTable: "ServiceOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOffers_Categories_CategoryId",
                table: "ServiceOffers",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ServiceOffers_ServiceOfferId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOffers_Categories_CategoryId",
                table: "ServiceOffers");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ServiceOffers_ServiceOfferId",
                table: "Reports",
                column: "ServiceOfferId",
                principalTable: "ServiceOffers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOffers_Categories_CategoryId",
                table: "ServiceOffers",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
