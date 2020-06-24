using Microsoft.Xna.Framework;
using System;

namespace MonoNet.GameSystems
{
    /// <summary>
    /// Class for getting information about the current time.
    /// </summary>
    public class Time : GameSystem
    {
        // Singleton pattern
        private static Time instance;

        // Gametime of this frame
        private GameTime currentTime;

        public Time()
        {
            instance = this;
        }

        /// <summary>
        /// Save the new GameTime to the currentTime
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            currentTime = gameTime;
        }

        /// <summary>
        /// Returns the time in seconds that elapsed since the previous frame.
        /// </summary>
        public static float Delta => instance.currentTime.ElapsedGameTime.Seconds;

        /// <summary>
        /// Returns the time elapsed since the application start.
        /// </summary>
        public static TimeSpan TotalGameTime => instance.currentTime.TotalGameTime;

        /// <summary>
        /// Returns the time since elapsed since the previous frame.
        /// </summary>
        public static TimeSpan ElapsedTime => instance.currentTime.ElapsedGameTime;
    }
}
