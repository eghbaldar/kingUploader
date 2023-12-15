using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KingUploader.Migrations
{
    public partial class add_filepartcount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FilePartCount",
                table: "Files",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePartCount",
                table: "Files");
        }
    }
}
