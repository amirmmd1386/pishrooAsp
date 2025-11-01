using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pishrooAsp.Migrations
{
    /// <inheritdoc />
    public partial class FixMessageRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductRequestId",
                table: "Message",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "Message",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Message_ProductRequestId",
                table: "Message",
                column: "ProductRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_ProductRequests_ProductRequestId",
                table: "Message",
                column: "ProductRequestId",
                principalTable: "ProductRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_ProductRequests_ProductRequestId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_ProductRequestId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "ProductRequestId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Message");
        }
    }
}
