using System;

namespace Frags.Core.Common
{
    public class GameRandom
    {
        private static readonly Random _random = new Random();

        public static int D4() => Between(1, 4);

        public static int D6() => Between(1, 6);

        public static int D8() => Between(1, 8);

        public static int D10() => Between(1, 10);

        public static int D12() => Between(1, 12);

        public static int D20() => Between(1, 20);

        public static int Between(int minimum, int maximum) =>
            _random.Next(minimum, maximum);
    }
}