using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WildStays.Migrations
{
    /// <inheritdoc />
    public partial class MutipleImagesssssssssas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Place",
                table: "Reservations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Place",
                table: "Reservations");
        }
    }
}
