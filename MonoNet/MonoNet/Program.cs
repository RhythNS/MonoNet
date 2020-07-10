using MonoNet.Testing;
using MonoNet.Testing.ECS;
using MonoNet.Testing.Physics;
using MonoNet.Testing.Tiled;
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
            using (var game = new PhysicTest())
                game.Run();
        }
    }
#endif
}
