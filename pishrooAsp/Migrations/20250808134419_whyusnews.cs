using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pishrooAsp.Migrations
{
    /// <inheritdoc />
    public partial class whyusnews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WhyUsTranslations_News_NewsId",
                table: "WhyUsTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_WhyUsTranslations_WhyUs_WhyUsItemId",
                table: "WhyUsTranslations");

            migrationBuilder.DropIndex(
                name: "IX_WhyUsTranslations_WhyUsItemId",
                table: "WhyUsTranslations");

            migrationBuilder.DropColumn(
                name: "WhyUsItemId",
                table: "WhyUsTranslations");

            migrationBuilder.RenameColumn(
                name: "NewsId",
                table: "WhyUsTranslations",
                newName: "whyusId");

            migrationBuilder.RenameIndex(
                name: "IX_WhyUsTranslations_NewsId",
                table: "WhyUsTranslations",
                newName: "IX_WhyUsTranslations_whyusId");

            migrationBuilder.AddForeignKey(
                name: "FK_WhyUsTranslations_WhyUs_whyusId",
                table: "WhyUsTranslations",
                column: "whyusId",
                principalTable: "WhyUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WhyUsTranslations_WhyUs_whyusId",
                table: "WhyUsTranslations");

            migrationBuilder.RenameColumn(
                name: "whyusId",
                table: "WhyUsTranslations",
                newName: "NewsId");

            migrationBuilder.RenameIndex(
                name: "IX_WhyUsTranslations_whyusId",
                table: "WhyUsTranslations",
                newName: "IX_WhyUsTranslations_NewsId");

            migrationBuilder.AddColumn<int>(
                name: "WhyUsItemId",
                table: "WhyUsTranslations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WhyUsTranslations_WhyUsItemId",
                table: "WhyUsTranslations",
                column: "WhyUsItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_WhyUsTranslations_News_NewsId",
                table: "WhyUsTranslations",
                column: "NewsId",
                principalTable: "News",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WhyUsTranslations_WhyUs_WhyUsItemId",
                table: "WhyUsTranslations",
                column: "WhyUsItemId",
                principalTable: "WhyUs",
                principalColumn: "Id");
        }
    }
}
