using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AJE.Service.NewsAnalyzer.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "analysis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    polarity = table.Column<int>(type: "integer", nullable: false),
                    polarityversion = table.Column<int>(type: "integer", nullable: false),
                    summaryversion = table.Column<int>(type: "integer", nullable: false),
                    summary = table.Column<string>(type: "text", nullable: false),
                    positivethingsversion = table.Column<int>(type: "integer", nullable: false),
                    positivethings = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analysis", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "analysis");
        }
    }
}
