using MonoNet.Testing;
using MonoNet.Testing.NetTest;
using System;

namespace MonoNet
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new MonoNet())
                game.Run();

            Environment.Exit(0);
        }
    }
#endif
}
