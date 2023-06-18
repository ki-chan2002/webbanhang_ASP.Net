using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab06_ASP.Net.Migrations
{
    /// <inheritdoc />
    public partial class allownullpassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Accounts",
                nullable: true, // Cho phép null
                oldNullable: false, // Giá trị null không được phép trước đó
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Accounts",
                nullable: false, // Giá trị null không được phép
                oldNullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
