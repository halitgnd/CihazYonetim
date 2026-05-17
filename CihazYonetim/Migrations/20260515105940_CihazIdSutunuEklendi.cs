using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CihazYonetim.Migrations
{
    /// <inheritdoc />
    public partial class CihazIdSutunuEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogTipi",
                table: "CihazLoglar");

            migrationBuilder.DropColumn(
                name: "Mesaj",
                table: "CihazLoglar");

            migrationBuilder.AddColumn<int>(
                name: "CihazId",
                table: "CihazLoglar",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Detay",
                table: "CihazLoglar",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Islem",
                table: "CihazLoglar",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CihazId",
                table: "CihazLoglar");

            migrationBuilder.DropColumn(
                name: "Detay",
                table: "CihazLoglar");

            migrationBuilder.DropColumn(
                name: "Islem",
                table: "CihazLoglar");

            migrationBuilder.AddColumn<string>(
                name: "LogTipi",
                table: "CihazLoglar",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mesaj",
                table: "CihazLoglar",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
