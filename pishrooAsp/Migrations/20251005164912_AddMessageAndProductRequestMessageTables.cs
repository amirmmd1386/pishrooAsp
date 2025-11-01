using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pishrooAsp.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageAndProductRequestMessageTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Message",
                newName: "CreatedAt");

            migrationBuilder.CreateTable(
                name: "ProductRequestMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRequestMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductRequestMessages_Message_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Message",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductRequestMessages_ProductRequests_ProductRequestId",
                        column: x => x.ProductRequestId,
                        principalTable: "ProductRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestMessages_MessageId",
                table: "ProductRequestMessages",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestMessages_ProductRequestId",
                table: "ProductRequestMessages",
                column: "ProductRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductRequestMessages");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Message",
                newName: "SentAt");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductRequestId",
                table: "Message",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
    }
}
