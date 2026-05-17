using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CihazYonetim.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDeviceRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddForeignKey(
                name: "FK_Cihazlar_Users_UserId",
                table: "Cihazlar",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cihazlar_Users_UserId",
                table: "Cihazlar");

            migrationBuilder.DropIndex(
                name: "IX_Cihazlar_UserId",
                table: "Cihazlar");

            migrationBuilder.DropColumn(
                name: "CihazId",
                table: "Users");
        }
    }
}
