using System;

namespace Frags.Core.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string actual, string expected) =>
            actual.Equals(expected, StringComparison.OrdinalIgnoreCase);

        public static bool ContainsIgnoreCase(this string actual, string subString) =>
            actual.IndexOf(subString, StringComparison.OrdinalIgnoreCase) > -1;
    }
}