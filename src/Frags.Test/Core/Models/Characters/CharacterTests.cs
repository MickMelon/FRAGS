using Frags.Core.Characters;
using Frags.Core.Common;
using Xunit;

namespace Frags.Test.Core.Models.Characters
{
    public class CharacterTests
    {
        #region Character Creation Tests
        [Fact]
        public void CanCreateCharacter()
        {
            var character = new Character(1, new User(2), true, "C", "Description", "Story");

            Assert.True(character.Id == 1
                && character.User.UserIdentifier == 2
                && character.Active == true
                && character.Name.Equals("C")
                && character.Description.Equals("Description")
                && character.Story.Equals("Story"));
        }
        #endregion

        #region Level and Experience Tests
        //[Fact]
        //public void GetLevel_ReturnCorrectLevel()
        //{
        //    // Arrange
        //    var character = new Character(1, "C");
        //    character.Experience = 1000;

        //    // Act
        //    int level = character.Level;
        //    int expected = Character.GetLevelFromExperience(character.Experience);

        //    // Assert
        //    Assert.Equal(expected, level);
        //}

        //[Fact]
        //public void SetExperience_SetsCorrectLevelToo()
        //{
        //    // Arrange
        //    var character = new Character(1, "C");

        //    // Act
        //    character.Experience = 5000;
        //    int expected = Character.GetLevelFromExperience(5000);
        //    int actual = character.Level;

        //    // Assert
        //    Assert.Equal(expected, actual);
        //}        
        #endregion
    }
}