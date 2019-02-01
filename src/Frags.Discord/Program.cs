using System;
using System.Threading.Tasks;

namespace Frags.Discord
{
    /// <summary>
    /// The default Program class for the Console project.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry point to the program.
        /// </summary>
        public static void Main(string[] args)
            => new Startup().StartAsync().GetAwaiter().GetResult();
    }
}