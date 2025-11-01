using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pishrooAsp.Migrations
{
    /// <inheritdoc />
    public partial class AddAboutUsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AboutUs");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "AboutUs");

            migrationBuilder.DropColumn(
                name: "CEOName",
                table: "AboutUs");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "AboutUs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AboutUs");

            migrationBuilder.DropColumn(
                name: "Slogan",
                table: "AboutUs");

            migrationBuilder.CreateTable(
                name: "AboutUsTranslation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slogan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CEOName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AboutUsId = table.Column<int>(type: "int", nullable: false),
                    LangId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutUsTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AboutUsTranslation_AboutUs_AboutUsId",
                        column: x => x.AboutUsId,
                        principalTable: "AboutUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AboutUsTranslation_Langs_LangId",
                        column: x => x.LangId,
                        principalTable: "Langs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AboutUsTranslation_AboutUsId",
                table: "AboutUsTranslation",
                column: "AboutUsId");

            migrationBuilder.CreateIndex(
                name: "IX_AboutUsTranslation_LangId",
                table: "AboutUsTranslation",
                column: "LangId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutUsTranslation");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AboutUs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "AboutUs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CEOName",
                table: "AboutUs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "AboutUs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AboutUs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slogan",
                table: "AboutUs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
