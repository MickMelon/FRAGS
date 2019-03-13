using Microsoft.EntityFrameworkCore.Migrations;

namespace Frags.Database.Migrations
{
    public partial class ActiveCharacter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Characters");

            migrationBuilder.CreateTable(
                name: "ActiveCharacters",
                columns: table => new
                {
                    UserIdentifier = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveCharacters", x => x.UserIdentifier);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserIdentifier",
                table: "Characters",
                column: "UserIdentifier",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_ActiveCharacters_UserIdentifier",
                table: "Characters",
                column: "UserIdentifier",
                principalTable: "ActiveCharacters",
                principalColumn: "UserIdentifier",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_ActiveCharacters_UserIdentifier",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "ActiveCharacters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_UserIdentifier",
                table: "Characters");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Characters",
                nullable: false,
                defaultValue: false);
        }
    }
}
