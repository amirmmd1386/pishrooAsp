using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pishrooAsp.Migrations
{
    /// <inheritdoc />
    public partial class whyus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WhyUs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhyUs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WhyUsTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewsId = table.Column<int>(type: "int", nullable: false),
                    LangId = table.Column<int>(type: "int", nullable: false),
                    WhyUsItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhyUsTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WhyUsTranslations_Langs_LangId",
                        column: x => x.LangId,
                        principalTable: "Langs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WhyUsTranslations_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WhyUsTranslations_WhyUs_WhyUsItemId",
                        column: x => x.WhyUsItemId,
                        principalTable: "WhyUs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WhyUsTranslations_LangId",
                table: "WhyUsTranslations",
                column: "LangId");

            migrationBuilder.CreateIndex(
                name: "IX_WhyUsTranslations_NewsId",
                table: "WhyUsTranslations",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_WhyUsTranslations_WhyUsItemId",
                table: "WhyUsTranslations",
                column: "WhyUsItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhyUsTranslations");

            migrationBuilder.DropTable(
                name: "WhyUs");
        }
    }
}
