using System.Threading.Tasks;
using Frags.Presentation.Controllers;

namespace Frags.Discord.Services
{
    public class SeedService
    {
        public static async Task Seed(StatisticController _statController)
        {
            await _statController.CreateAttributeAsync("Strength");
            await _statController.CreateAttributeAsync("Perception");
            await _statController.CreateAttributeAsync("Endurance");
            await _statController.CreateAttributeAsync("Charisma");
            await _statController.CreateAttributeAsync("Intelligence");
            await _statController.CreateAttributeAsync("Agility");
            await _statController.CreateAttributeAsync("Luck");

            await _statController.AddAliasAsync("Strength", "STR");
            await _statController.AddAliasAsync("Perception", "PER");
            await _statController.AddAliasAsync("Endurance", "END");
            await _statController.AddAliasAsync("Charisma", "CHA");
            await _statController.AddAliasAsync("Intelligence", "INT");
            await _statController.AddAliasAsync("Agility", "AGI");
            await _statController.AddAliasAsync("Luck", "LCK");

            await _statController.CreateSkillAsync("Barter", "Charisma");
            await _statController.CreateSkillAsync("Energy Weapons", "Perception");
            await _statController.CreateSkillAsync("Explosives", "Perception");
            await _statController.CreateSkillAsync("Guns", "Agility");
            await _statController.CreateSkillAsync("Lockpick", "Perception");
            await _statController.CreateSkillAsync("Medicine", "Intelligence");
            await _statController.CreateSkillAsync("Melee Weapons", "Strength");
            await _statController.CreateSkillAsync("Repair", "Intelligence");
            await _statController.CreateSkillAsync("Science", "Intelligence");
            await _statController.CreateSkillAsync("Sneak", "Agility");
            await _statController.CreateSkillAsync("Speech", "Charisma");
            await _statController.CreateSkillAsync("Survival", "Endurance");
            await _statController.CreateSkillAsync("Unarmed", "Endurance");
        }
    }
}