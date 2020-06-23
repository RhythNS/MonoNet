using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoNet.GameSystem
{
    /// <summary>
    /// Wrapper for Keyboard state for ease of use.
    /// </summary>
    public class Input : GameSystem
    {
        /// <summary>
        /// Singleton which can only be accessed from inside this class.
        /// </summary>
        private static Input instance;

        private KeyboardState prev, current;

        public Input()
        {
            instance = this;
            current = Keyboard.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            // Set prev to current and set the current to the new Keyboard state.
            prev = current;
            current = Keyboard.GetState();
        }

        /// <summary>
        /// Returns true if key was pressed this frame.
        /// </summary>
        public static bool IsKeyDownThisFrame(Keys key) => instance.current.IsKeyDown(key) && instance.prev.IsKeyUp(key);

        /// <summary>
        /// Returns true if key was just unpressed this frame.
        /// </summary>
        public static bool IsKeyUpThisFrame(Keys key) => instance.current.IsKeyUp(key) && instance.prev.IsKeyDown(key);

        /// <summary>
        /// Returns true if the key is currently pressed.
        /// </summary>
        public static bool KeyDown(Keys key) => instance.current.IsKeyDown(key);

        /// <summary>
        /// Returns true if the is currently not pressed.
        /// </summary>
        public static bool KeyUp(Keys key) => instance.current.IsKeyUp(key);
        
    }
}
