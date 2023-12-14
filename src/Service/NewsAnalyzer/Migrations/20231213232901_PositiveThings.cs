using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJE.Service.NewsAnalyzer.Migrations
{
    /// <inheritdoc />
    public partial class PositiveThings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "positivethings",
                table: "analyses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "positivethingsversion",
                table: "analyses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "positivethings",
                table: "analyses");

            migrationBuilder.DropColumn(
                name: "positivethingsversion",
                table: "analyses");
        }
    }
}
