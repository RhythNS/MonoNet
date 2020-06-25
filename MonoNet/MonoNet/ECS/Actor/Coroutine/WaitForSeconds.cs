using MonoNet.GameSystems;

namespace MonoNet.ECS
{
    /// <summary>
    /// Yields the coroutine for a specified amount of seconds.
    /// </summary>
    public class WaitForSeconds : YieldInstruction
    {
        private readonly float seconds;
        private float timer;

        /// <summary>
        /// Waits for given seconds.
        /// </summary>
        /// <param name="seconds">Time in seconds.</param>
        public WaitForSeconds(float seconds)
        {
            this.seconds = seconds;
        }

        public override bool IsDone()
        {
            timer += Time.Delta;
            return timer > seconds;
        }
    }
}
