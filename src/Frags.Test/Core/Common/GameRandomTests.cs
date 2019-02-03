using System;
using Frags.Core.Common;
using Xunit;

namespace Frags.Test.Core.Common
{
    public class GameRandomTests
    {
        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 10)]
        [InlineData(11, 50)]
        public void Between_ValidValues_ReturnBetweenValues(int minimum, int maximum)
        {
            int result = GameRandom.Between(minimum, maximum);
            Assert.True(result >= minimum && result <= maximum);
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(2, 1)]
        public void Between_MaximumLowerThanMinimum_ThrowException(int minimum, int maximum)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GameRandom.Between(minimum, maximum));
        }

        [Fact]
        public void D4_ReturnsBetweenOneAndFour()
        {
            int result = GameRandom.D4();
            Assert.True(result >= 1 && result <= 4);
        }

        [Fact]
        public void D6_ReturnsBetweenOneAndSix()
        {
            int result = GameRandom.D6();
            Assert.True(result >= 1 && result <= 6);
        }

        [Fact]
        public void D8_ReturnsBetweenOneAndEight()
        {
            int result = GameRandom.D8();
            Assert.True(result >= 1 && result <= 8);
        }

        [Fact]
        public void D10_ReturnsBetweenOneAndTen()
        {
            int result = GameRandom.D10();
            Assert.True(result >= 1 && result <= 10);
        }

        [Fact]
        public void D12_ReturnsBetweenOneAndTwelve()
        {
            int result = GameRandom.D12();
            Assert.True(result >= 1 && result <= 12);
        }

        [Fact]
        public void D20_ReturnsBetweenOneAndTwenty()
        {
            int result = GameRandom.D20();
            Assert.True(result >= 1 && result <= 20);
        }
    }
}