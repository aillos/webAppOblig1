using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WildStays.Migrations
{
    /// <inheritdoc />
    public partial class MutipleImagessss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Listings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Listings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
