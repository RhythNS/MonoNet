namespace MonoNet.ECS
{
    /// <summary>
    /// Yield instructions for Coroutines.
    /// </summary>
    public abstract class YieldInstruction
    {
        /// <summary>
        /// Returns true when the yield instruction is finished, false when it is still running.
        /// </summary>
        public abstract bool IsDone();
    }
}
