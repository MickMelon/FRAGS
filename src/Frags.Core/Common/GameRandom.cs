using System;

namespace Frags.Core.Common
{
    /// <summary>
    /// Contains the single Random for "true RNG".
    /// </summary>
    public static class GameRandom
    {
        /// <summary>
        /// The Random object.
        /// </summary>
        private static readonly Random _random = new Random();

        /// <summary>
        /// Rolls a four sided die.
        /// </summary>
        public static int D4() => Between(1, 4);

        /// <summary>
        /// Rolls a six sided die.
        /// </summary>
        public static int D6() => Between(1, 6);

        /// <summary>
        /// Rolls an eight sided die.
        /// </summary>
        public static int D8() => Between(1, 8);

        /// <summary>
        /// Rolls a ten sided die.
        /// </summary>
        public static int D10() => Between(1, 10);

        /// <summary>
        /// Rolls a twelve sided die.
        /// </summary>
        public static int D12() => Between(1, 12);

        /// <summary>
        /// Rolls a twenty sided die.
        /// </summary>
        public static int D20() => Between(1, 20);

        /// <summary>
        /// Gets a random number between two inclusive numbers.
        /// </summary>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        /// <returns>A random number between minimum and maximum.</returns>
        public static int Between(int minimum, int maximum) =>
            _random.Next(minimum, maximum);
    }
}