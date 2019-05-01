using Microsoft.EntityFrameworkCore.Migrations;

namespace Frags.Database.Migrations
{
    public partial class Campaigns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RollOptionsDto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RollStrategy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RollOptionsDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatisticOptionsDto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProgressionStrategy = table.Column<string>(nullable: true),
                    AttributeMax = table.Column<int>(nullable: false),
                    SkillMax = table.Column<int>(nullable: false),
                    InitialSetupMaxLevel = table.Column<int>(nullable: false),
                    InitialSkillPoints = table.Column<int>(nullable: false),
                    InitialSkillMax = table.Column<int>(nullable: false),
                    InitialSkillMin = table.Column<int>(nullable: false),
                    InitialSkillsAtMax = table.Column<int>(nullable: false),
                    InitialSkillsProficient = table.Column<int>(nullable: false),
                    InitialAttributePoints = table.Column<int>(nullable: false),
                    InitialAttributeMax = table.Column<int>(nullable: false),
                    InitialAttributeMin = table.Column<int>(nullable: false),
                    InitialAttributesAtMax = table.Column<int>(nullable: false),
                    InitialAttributesProficient = table.Column<int>(nullable: false),
                    SkillPointsOnLevelUp = table.Column<int>(nullable: false),
                    AttributePointsOnLevelUp = table.Column<int>(nullable: false),
                    ProficientAttributeMultiplier = table.Column<double>(nullable: false),
                    ProficientSkillMultiplier = table.Column<double>(nullable: false),
                    ExpMessageLengthDivisor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticOptionsDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatisticValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<int>(nullable: false),
                    IsProficient = table.Column<bool>(nullable: false),
                    Proficiency = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticValue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerUserIdentifier = table.Column<ulong>(nullable: false),
                    RollOptionsId = table.Column<int>(nullable: true),
                    StatisticOptionsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaigns_RollOptionsDto_RollOptionsId",
                        column: x => x.RollOptionsId,
                        principalTable: "RollOptionsDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Campaigns_StatisticOptionsDto_StatisticOptionsId",
                        column: x => x.StatisticOptionsId,
                        principalTable: "StatisticOptionsDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChannelDto",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CampaignDtoId = table.Column<int>(nullable: true),
                    StatisticOptionsDtoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelDto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelDto_Campaigns_CampaignDtoId",
                        column: x => x.CampaignDtoId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChannelDto_StatisticOptionsDto_StatisticOptionsDtoId",
                        column: x => x.StatisticOptionsDtoId,
                        principalTable: "StatisticOptionsDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserIdentifier = table.Column<ulong>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Story = table.Column<string>(nullable: true),
                    Experience = table.Column<int>(nullable: false),
                    Money = table.Column<int>(nullable: false),
                    AttributePoints = table.Column<int>(nullable: false),
                    SkillPoints = table.Column<int>(nullable: false),
                    CampaignDtoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Campaigns_CampaignDtoId",
                        column: x => x.CampaignDtoId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Effects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    OwnerUserIdentifier = table.Column<ulong>(nullable: false),
                    CampaignDtoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Effects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Effects_Campaigns_CampaignDtoId",
                        column: x => x.CampaignDtoId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Aliases = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CampaignDtoId = table.Column<int>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    AttributeId = table.Column<int>(nullable: true),
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
                    table.ForeignKey(
                        name: "FK_Statistics_Campaigns_CampaignDtoId",
                        column: x => x.CampaignDtoId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserIdentifier = table.Column<ulong>(nullable: false),
                    ActiveCharacterId = table.Column<int>(nullable: true)
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
                    CharacterId = table.Column<int>(nullable: false),
                    EffectId = table.Column<int>(nullable: false)
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
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StatisticId = table.Column<int>(nullable: true),
                    StatisticValueId = table.Column<int>(nullable: true),
                    CharacterDtoId = table.Column<int>(nullable: true),
                    EffectDtoId = table.Column<int>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Moderator",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    CampaignId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moderator", x => new { x.UserId, x.CampaignId });
                    table.ForeignKey(
                        name: "FK_Moderator_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Moderator_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_RollOptionsId",
                table: "Campaigns",
                column: "RollOptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_StatisticOptionsId",
                table: "Campaigns",
                column: "StatisticOptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelDto_CampaignDtoId",
                table: "ChannelDto",
                column: "CampaignDtoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelDto_StatisticOptionsDtoId",
                table: "ChannelDto",
                column: "StatisticOptionsDtoId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CampaignDtoId",
                table: "Characters",
                column: "CampaignDtoId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectMapping_CharacterId",
                table: "EffectMapping",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Effects_CampaignDtoId",
                table: "Effects",
                column: "CampaignDtoId");

            migrationBuilder.CreateIndex(
                name: "IX_Moderator_CampaignId",
                table: "Moderator",
                column: "CampaignId");

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
                name: "IX_Statistics_CampaignDtoId",
                table: "Statistics",
                column: "CampaignDtoId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ActiveCharacterId",
                table: "Users",
                column: "ActiveCharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelDto");

            migrationBuilder.DropTable(
                name: "EffectMapping");

            migrationBuilder.DropTable(
                name: "Moderator");

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

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "RollOptionsDto");

            migrationBuilder.DropTable(
                name: "StatisticOptionsDto");
        }
    }
}
