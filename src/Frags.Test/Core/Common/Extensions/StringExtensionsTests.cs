using Xunit;
using Frags.Core.Common.Extensions;

namespace Frags.Test.Core.Common.Extensions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("a", "a")]
        [InlineData("a", "A")]
        [InlineData("A", "a")]
        [InlineData("A", "A")]
        [InlineData("1", "1")]
        public void EqualIgnoreCase_EqualValues_ReturnTrue(string value1, string value2)
        {
            Assert.True(value1.EqualsIgnoreCase(value2));
        }

        [Theory]
        [InlineData("a", "b")]
        [InlineData("a", "B")]
        [InlineData("A", "b")]
        [InlineData("A", "B")]
        [InlineData("1", "2")]
        public void EqualIgnoreCase_NotEqualValues_ReturnFalse(string value1, string value2)
        {
            Assert.False(value1.EqualsIgnoreCase(value2));
        }
    }
}