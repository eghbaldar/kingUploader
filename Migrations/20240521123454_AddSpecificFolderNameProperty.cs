using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KingUploader.Migrations
{
    public partial class AddSpecificFolderNameProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SpecificFolderName",
                table: "MultiFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecificFolderName",
                table: "MultiFiles");
        }
    }
}
