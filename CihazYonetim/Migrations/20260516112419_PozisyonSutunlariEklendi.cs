using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CihazYonetim.Migrations
{
    /// <inheritdoc />
    public partial class PozisyonSutunlariEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PositionX",
                table: "Cihazlar",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PositionY",
                table: "Cihazlar",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionX",
                table: "Cihazlar");

            migrationBuilder.DropColumn(
                name: "PositionY",
                table: "Cihazlar");
        }
    }
}
