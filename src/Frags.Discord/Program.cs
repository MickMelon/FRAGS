using System;
using System.Threading.Tasks;

namespace Frags.Discord
{
    public static class Program
    {
        public static void Main(string[] args)
            => new Startup().StartAsync().GetAwaiter().GetResult();
    }
}