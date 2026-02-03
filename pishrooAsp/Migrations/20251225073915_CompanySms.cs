using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pishrooAsp.Migrations
{
    /// <inheritdoc />
    public partial class CompanySms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageTemplates_Companies_CompanyId",
                table: "MessageTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_SentMessages_Companies_CompanyId",
                table: "SentMessages");

            migrationBuilder.DropIndex(
                name: "IX_Companies_Name",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Companies",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Companies",
                newName: "Mobile");

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanySmsLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    SmsTemplateId = table.Column<int>(type: "int", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    ProviderResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySmsLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanySmsLogs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanySmsLogs_SmsTemplates_SmsTemplateId",
                        column: x => x.SmsTemplateId,
                        principalTable: "SmsTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanySmsLogs_CompanyId",
                table: "CompanySmsLogs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySmsLogs_SmsTemplateId",
                table: "CompanySmsLogs",
                column: "SmsTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageTemplates_Company_CompanyId",
                table: "MessageTemplates",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SentMessages_Company_CompanyId",
                table: "SentMessages",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageTemplates_Company_CompanyId",
                table: "MessageTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_SentMessages_Company_CompanyId",
                table: "SentMessages");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "CompanySmsLogs");

            migrationBuilder.DropTable(
                name: "SmsTemplates");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Companies",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "Mobile",
                table: "Companies",
                newName: "Email");

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Companies",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                table: "Companies",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageTemplates_Companies_CompanyId",
                table: "MessageTemplates",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SentMessages_Companies_CompanyId",
                table: "SentMessages",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }
    }
}
