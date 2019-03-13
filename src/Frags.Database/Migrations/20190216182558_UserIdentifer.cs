using Microsoft.EntityFrameworkCore.Migrations;

namespace Frags.Database.Migrations
{
    public partial class UserIdentifer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Characters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<ulong>(
                name: "UserIdentifier",
                table: "Characters",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "UserIdentifier",
                table: "Characters");
        }
    }
}
