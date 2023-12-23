using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJE.Service.NewsAnalyzer.Migrations
{
    /// <inheritdoc />
    public partial class LocCorpOrg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "corporations",
                table: "analysis",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "corporationsversion",
                table: "analysis",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "locations",
                table: "analysis",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "locationsversion",
                table: "analysis",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "organizations",
                table: "analysis",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "organizationsversion",
                table: "analysis",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "corporations",
                table: "analysis");

            migrationBuilder.DropColumn(
                name: "corporationsversion",
                table: "analysis");

            migrationBuilder.DropColumn(
                name: "locations",
                table: "analysis");

            migrationBuilder.DropColumn(
                name: "locationsversion",
                table: "analysis");

            migrationBuilder.DropColumn(
                name: "organizations",
                table: "analysis");

            migrationBuilder.DropColumn(
                name: "organizationsversion",
                table: "analysis");
        }
    }
}
