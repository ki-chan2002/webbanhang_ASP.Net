using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab06_ASP.Net.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Coffees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Coffees");
        }
    }
}
