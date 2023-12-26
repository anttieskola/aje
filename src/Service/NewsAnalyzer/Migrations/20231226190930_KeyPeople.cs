using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJE.Service.NewsAnalyzer.Migrations
{
    /// <inheritdoc />
    public partial class KeyPeople : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "keypeople",
                table: "analysis",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "keypeopleversion",
                table: "analysis",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "keypeople",
                table: "analysis");

            migrationBuilder.DropColumn(
                name: "keypeopleversion",
                table: "analysis");
        }
    }
}
