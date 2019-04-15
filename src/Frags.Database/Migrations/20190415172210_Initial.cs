using Microsoft.EntityFrameworkCore.Migrations;

namespace Frags.Database.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserIdentifier = table.Column<ulong>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Story = table.Column<string>(nullable: true),
                    Experience = table.Column<int>(nullable: false),
                    Money = table.Column<int>(nullable: false),
                    AttributePoints = table.Column<int>(nullable: false),
                    SkillPoints = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Effects",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    OwnerUserIdentifier = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Effects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Aliases = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    AttributeId = table.Column<string>(nullable: true),
                    MinimumValue = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statistics_Statistics_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Statistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatisticValue",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Value = table.Column<int>(nullable: false),
                    IsProficient = table.Column<bool>(nullable: false),
                    Proficiency = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticValue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserIdentifier = table.Column<ulong>(nullable: false),
                    ActiveCharacterId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Characters_ActiveCharacterId",
                        column: x => x.ActiveCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EffectMapping",
                columns: table => new
                {
                    CharacterId = table.Column<string>(nullable: false),
                    EffectId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectMapping", x => new { x.EffectId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_EffectMapping_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EffectMapping_Effects_EffectId",
                        column: x => x.EffectId,
                        principalTable: "Effects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatisticMapping",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    StatisticId = table.Column<string>(nullable: true),
                    StatisticValueId = table.Column<string>(nullable: true),
                    CharacterDtoId = table.Column<string>(nullable: true),
                    EffectDtoId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatisticMapping_Characters_CharacterDtoId",
                        column: x => x.CharacterDtoId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatisticMapping_Effects_EffectDtoId",
                        column: x => x.EffectDtoId,
                        principalTable: "Effects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatisticMapping_Statistics_StatisticId",
                        column: x => x.StatisticId,
                        principalTable: "Statistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatisticMapping_StatisticValue_StatisticValueId",
                        column: x => x.StatisticValueId,
                        principalTable: "StatisticValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EffectMapping_CharacterId",
                table: "EffectMapping",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticMapping_CharacterDtoId",
                table: "StatisticMapping",
                column: "CharacterDtoId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticMapping_EffectDtoId",
                table: "StatisticMapping",
                column: "EffectDtoId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticMapping_StatisticId",
                table: "StatisticMapping",
                column: "StatisticId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticMapping_StatisticValueId",
                table: "StatisticMapping",
                column: "StatisticValueId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_AttributeId",
                table: "Statistics",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ActiveCharacterId",
                table: "Users",
                column: "ActiveCharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EffectMapping");

            migrationBuilder.DropTable(
                name: "StatisticMapping");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Effects");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "StatisticValue");

            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
