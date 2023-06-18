using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab06_ASP.Net.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Coffees",
                nullable: true,  // Chuyển thuộc tính thành nullable
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);  // Xóa ràng buộc NOT NULL

            // Các tác vụ migration khác (nếu có)
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Coffees",
                type: "nvarchar(max)",
                nullable: false,  // Đặt lại ràng buộc NOT NULL
                oldClrType: typeof(string),
                oldNullable: true);  // Chuyển thuộc tính thành non-nullable

            // Các tác vụ rollback migration khác (nếu có)
        }
    }
}
