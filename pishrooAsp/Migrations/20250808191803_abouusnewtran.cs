using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pishrooAsp.Migrations
{
    /// <inheritdoc />
    public partial class abouusnewtran : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AboutUsTranslation_AboutUs_AboutUsId",
                table: "AboutUsTranslation");

            migrationBuilder.DropForeignKey(
                name: "FK_AboutUsTranslation_Langs_LangId",
                table: "AboutUsTranslation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AboutUsTranslation",
                table: "AboutUsTranslation");

            migrationBuilder.RenameTable(
                name: "AboutUsTranslation",
                newName: "AboutUsTranslations");

            migrationBuilder.RenameIndex(
                name: "IX_AboutUsTranslation_LangId",
                table: "AboutUsTranslations",
                newName: "IX_AboutUsTranslations_LangId");

            migrationBuilder.RenameIndex(
                name: "IX_AboutUsTranslation_AboutUsId",
                table: "AboutUsTranslations",
                newName: "IX_AboutUsTranslations_AboutUsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AboutUsTranslations",
                table: "AboutUsTranslations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AboutUsTranslations_AboutUs_AboutUsId",
                table: "AboutUsTranslations",
                column: "AboutUsId",
                principalTable: "AboutUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AboutUsTranslations_Langs_LangId",
                table: "AboutUsTranslations",
                column: "LangId",
                principalTable: "Langs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AboutUsTranslations_AboutUs_AboutUsId",
                table: "AboutUsTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_AboutUsTranslations_Langs_LangId",
                table: "AboutUsTranslations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AboutUsTranslations",
                table: "AboutUsTranslations");

            migrationBuilder.RenameTable(
                name: "AboutUsTranslations",
                newName: "AboutUsTranslation");

            migrationBuilder.RenameIndex(
                name: "IX_AboutUsTranslations_LangId",
                table: "AboutUsTranslation",
                newName: "IX_AboutUsTranslation_LangId");

            migrationBuilder.RenameIndex(
                name: "IX_AboutUsTranslations_AboutUsId",
                table: "AboutUsTranslation",
                newName: "IX_AboutUsTranslation_AboutUsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AboutUsTranslation",
                table: "AboutUsTranslation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AboutUsTranslation_AboutUs_AboutUsId",
                table: "AboutUsTranslation",
                column: "AboutUsId",
                principalTable: "AboutUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AboutUsTranslation_Langs_LangId",
                table: "AboutUsTranslation",
                column: "LangId",
                principalTable: "Langs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
