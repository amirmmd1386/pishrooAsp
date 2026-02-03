using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pishrooAsp.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsToSmsModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "GroupSmsCampaigns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "GroupSmsCampaigns",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "GroupSmsCampaigns");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "GroupSmsCampaigns");
        }
    }
}
