using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CihazYonetim.Migrations
{
    /// <inheritdoc />
    public partial class CokluCihazGuncellemesi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cihazlar_UserId",
                table: "Cihazlar");

            migrationBuilder.DropColumn(
                name: "CihazId",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Cihazlar_UserId",
                table: "Cihazlar",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cihazlar_UserId",
                table: "Cihazlar");

            migrationBuilder.AddColumn<int>(
                name: "CihazId",
                table: "Users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cihazlar_UserId",
                table: "Cihazlar",
                column: "UserId",
                unique: true);
        }
    }
}
