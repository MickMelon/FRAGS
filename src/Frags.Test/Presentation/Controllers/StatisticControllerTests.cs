using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Common;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Statistics;
using Frags.Core.Statistics;
using Frags.Presentation.Controllers;
using Frags.Presentation.Results;
using Frags.Presentation.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace Frags.Test.Presentation.Controllers
{
    public class StatisticControllerTests
    {
        private readonly ITestOutputHelper output;

        public StatisticControllerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task CreateAttributeAsync_ValidInput_ReturnSuccess()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null);

            var result = await controller.CreateAttributeAsync("Wisdom");

            Assert.Equal(StatisticResult.StatisticCreatedSuccessfully(), result);
        }

        [Fact]
        public async Task CreateAttributeAsync_AlreadyExists_ReturnNameAlreadyExists()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null);

            var result = await controller.CreateAttributeAsync("Strength");

            Assert.Equal(StatisticResult.NameAlreadyExists(), result);
        }

        [Fact]
        public async Task CreateSkillAsync_ValidInput_ReturnSuccess()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null);

            var result = await controller.CreateSkillAsync("Intimidation", "Strength");

            Assert.Equal(StatisticResult.StatisticCreatedSuccessfully(), result);
        }

        [Fact]
        public async Task CreateSkillAsync_AlreadyExists_ReturnNameAlreadyExists()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null);

            var result = await controller.CreateSkillAsync("Powerlifting", "Strength");

            Assert.Equal(StatisticResult.NameAlreadyExists(), result);
        }

        [Fact]
        public async Task CreateSkillAsync_InvalidAttributeName_ReturnCreationFailed()
        {
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var controller = new StatisticController(charProvider, statProvider, null);

            var result = await controller.CreateSkillAsync("Intimidation", "STR");

            Assert.Equal(StatisticResult.StatisticCreationFailed(), result);
        }

        #region 
        [Fact]
        public async Task SetStatisticAsync_ValidInput_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            
            var statOptions = new StatisticOptions
            {
                InitialAttributeMin = 1,
                InitialAttributeMax = 10,
                InitialAttributePoints = 40,
                InitialAttributesAtMax = 7
            };

            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, statOptions));
            var character = await charProvider.GetActiveCharacterAsync(1);

            // Act
            await controller.SetStatisticAsync(1, "strength", 10);
            await controller.SetStatisticAsync(1, "perception", 2);
            await controller.SetStatisticAsync(1, "endurance", 6);
            await controller.SetStatisticAsync(1, "charisma", 6);
            await controller.SetStatisticAsync(1, "intelligence", 6);
            await controller.SetStatisticAsync(1, "agility", 5);
            await controller.SetStatisticAsync(1, "luck", 5);

            // Assert
            Statistic str = await statProvider.GetStatisticAsync("strength"),
            per = await statProvider.GetStatisticAsync("perception"),
            end = await statProvider.GetStatisticAsync("endurance"),
            cha = await statProvider.GetStatisticAsync("charisma"),
            inte = await statProvider.GetStatisticAsync("intelligence"),
            agi = await statProvider.GetStatisticAsync("agility"),
            lck = await statProvider.GetStatisticAsync("luck");

            Assert.True(character.GetStatistic(str).Value.Equals(10) &&
            character.GetStatistic(per).Value.Equals(2) &&
            character.GetStatistic(end).Value.Equals(6) &&
            character.GetStatistic(cha).Value.Equals(6) &&
            character.GetStatistic(inte).Value.Equals(6) && 
            character.GetStatistic(agi).Value.Equals(5) &&
            character.GetStatistic(lck).Value.Equals(5));
        }

        [Fact]
        public async Task SetProficiencyAsync_ValidInput_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            
            var statOptions = new StatisticOptions
            {
                InitialAttributeMin = 1,
                InitialAttributeMax = 10,
                InitialAttributePoints = 40,
                InitialAttributesAtMax = 7,
                InitialAttributesProficient = 1
            };

            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, statOptions));
            var character = await charProvider.GetActiveCharacterAsync(1);
            Statistic str = await statProvider.GetStatisticAsync("strength");
            await controller.SetStatisticAsync(1, "strength", 10);

            // Act
            await controller.SetProficiencyAsync(1, "strength", true);

            // Assert
            Assert.True(character.GetStatistic(str).IsProficient);
        }

        [Fact]
        public async Task SetProficiencyAsync_TooMany_ReturnFailure()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            
            var statOptions = new StatisticOptions
            {
                InitialAttributeMin = 1,
                InitialAttributeMax = 10,
                InitialAttributePoints = 40,
                InitialAttributesAtMax = 7,
                InitialAttributesProficient = 0
            };

            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, statOptions));
            var character = await charProvider.GetActiveCharacterAsync(1);
            await controller.SetStatisticAsync(1, "strength", 10);

            // Act
            var result = await controller.SetProficiencyAsync(1, "strength", true);

            // Assert
            Assert.Equal(GenericResult.NotEnoughPoints(), result);
        }

        [Fact]
        public async Task SetProficiencyAsync_SetFalse_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            
            var statOptions = new StatisticOptions
            {
                InitialAttributeMin = 1,
                InitialAttributeMax = 10,
                InitialAttributePoints = 40,
                InitialAttributesAtMax = 7,
                InitialAttributesProficient = 0
            };

            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, statOptions));
            var character = await charProvider.GetActiveCharacterAsync(1);
            await controller.SetStatisticAsync(1, "strength", 10);

            // Act
            var result = await controller.SetProficiencyAsync(1, "strength", false);

            // Assert
            Assert.Equal(StatisticResult.StatisticSetSucessfully(), result);
        }

        [Fact]
        public async Task SetStatisticAsync_NotEnoughPoints_ReturnNotEnoughPoints()
        {
            // Arrange
            int points = 1;
            int luckScore = 2;

            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                InitialAttributeMax = 10,
                InitialAttributePoints = points
            };

            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, statOptions));

            // Act
            var result = await controller.SetStatisticAsync(1, "luck", 2);

            // Assert
            // TODO: update unit tests to match new result
            Assert.Equal(GenericResult.Failure(
                string.Format(Messages.STAT_NOT_ENOUGH_POINTS, luckScore, points)), result);
        }

        [Fact]
        public async Task SetStatisticAsync_AboveMaximum_ReturnTooHigh()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                InitialAttributeMax = 10,
                InitialAttributePoints = 12,
            };

            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, statOptions));

            // Act
            var result = await controller.SetStatisticAsync(1, "strength", 11);

            // Assert
            Assert.Equal(GenericResult.ValueTooHigh(), result);
        }

        [Fact]
        public async Task SetStatisticAsync_BelowMinimum_ReturnTooLow()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                InitialAttributeMin = 1,
                InitialAttributeMax = 10,
                InitialAttributePoints = 10
            };

            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, statOptions));

            // Act
            var result = await controller.SetStatisticAsync(1, "strength", 0);

            // Assert
            Assert.Equal(GenericResult.ValueTooLow(), result);
        }

        [Fact]
        public async Task SetStatisticAsync_TooManyAtMax_ReturnTooManyAtMax()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                InitialAttributeMax = 2,
                InitialAttributePoints = 6,
                InitialAttributesAtMax = 2
            };

            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, statOptions));

            // Act
            await controller.SetStatisticAsync(1, "strength", 2);
            await controller.SetStatisticAsync(1, "perception", 2);
            var result = await controller.SetStatisticAsync(1, "endurance", 2);

            // Assert
            Assert.Equal(StatisticResult.TooManyAtMax(2), result);
        }

        [Fact]
        public async Task SetStatisticAsync_LevelTooHighButAttributesUnset_ReturnSuccess()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var character = await charProvider.CreateCharacterAsync(100, "TooHigh");
            // the important bit
            character.Experience = 50000;

            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                InitialAttributeMin = 1,
                InitialAttributeMax = 10,
                InitialAttributePoints = 10,
                InitialSetupMaxLevel = 1
            };

            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, statOptions));

            // Act
            var result = await controller.SetStatisticAsync(100, "strength", 5);

            // Assert
            Assert.Equal(StatisticResult.StatisticSetSucessfully(), result);
        }

        [Fact]
        public async Task SetStatisticAsync_LevelTooHigh_ReturnLevelTooHigh()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var character = await charProvider.CreateCharacterAsync(100, "TooHigh");

            var statProvider = new MockStatisticProvider();
            var statOptions = new StatisticOptions
            {
                InitialAttributeMin = 1,
                InitialAttributeMax = 10,
                InitialAttributePoints = 40,
                InitialSetupMaxLevel = 1
            };

            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, statOptions));

            // Act
            await controller.SetStatisticAsync(100, "strength", 10);
            await controller.SetStatisticAsync(100, "perception", 2);
            await controller.SetStatisticAsync(100, "endurance", 6);
            await controller.SetStatisticAsync(100, "charisma", 6);
            await controller.SetStatisticAsync(100, "intelligence", 6);
            await controller.SetStatisticAsync(100, "agility", 5);
            await controller.SetStatisticAsync(100, "luck", 5);

            character.Experience = 50000;
            await charProvider.UpdateCharacterAsync(character);
            var result = await controller.SetStatisticAsync(100, "strength", 4);

            // Assert
            Assert.Equal(CharacterResult.LevelTooHigh(), result);
        }

        [Fact]
        public async Task SetStatisticAsync_InvalidCharacterId_ReturnCharacterNotFound()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            StatisticOptions statOptions = new StatisticOptions();

            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, statOptions));

            // Act
            var result = await controller.SetStatisticAsync(ulong.MaxValue, "strength", 5);

            // Assert
            Assert.Equal(CharacterResult.CharacterNotFound(), result);
        }

        [Fact]
        public async Task SetStatisticAsync_InvalidStatisticName_ReturnStatisticNotFound()
        {
            // Arrange
            var charProvider = new MockCharacterProvider();
            var statProvider = new MockStatisticProvider();
            StatisticOptions statOptions = new StatisticOptions();

            var controller = new StatisticController(charProvider, statProvider, new GenericProgressionStrategy(statProvider, statOptions));

            // Act
            var result = await controller.SetStatisticAsync(1, "invalid", 5);

            // Assert
            Assert.Equal(StatisticResult.StatisticNotFound(), result);
        }
        #endregion
    }
}