using MonoNet.Util;
using System;

namespace MonoNet.Graphics
{
    /// <summary>
    /// Represents an animation.
    /// </summary>
    /// <typeparam name="T">The keyframe type.</typeparam>
    public class Animation<T>
    {
        public enum PlaybackMode
        {
            Normal, Reversed, NormalLoop, ReversedLoop, PingPongLoop
        }

        private T[] keyFrames;
        private float keyframeDuration;
        private PlaybackMode playbackMode;

        public Animation(T[] keyFrames, float keyframeDuration, PlaybackMode playbackMode = PlaybackMode.Normal)
        {
            this.keyFrames = keyFrames;
            this.keyframeDuration = keyframeDuration;
            this.playbackMode = playbackMode;
        }

        /// <summary>
        /// Gets a keyframe with total elapsed time.
        /// </summary>
        /// <param name="time">Time in total elapsed time. Not delta time!</param>
        /// <returns>The current keyframe.</returns>
        public T GetKeyframe(float time)
        {
            if (time < 0)
                time = 0;

            int at = (int)(time / keyframeDuration);
            switch (playbackMode)
            {
                case PlaybackMode.Normal:
                    return keyFrames[Math.Min(at, keyFrames.Length - 1)];

                case PlaybackMode.Reversed:
                    return keyFrames[Math.Max(keyFrames.Length - 1 - at, 0)];

                case PlaybackMode.NormalLoop:
                    at %= keyFrames.Length;
                    goto case PlaybackMode.Normal;

                case PlaybackMode.ReversedLoop:
                    at %= keyFrames.Length;
                    goto case PlaybackMode.Reversed;

                case PlaybackMode.PingPongLoop:
                    at %= keyFrames.Length * 2;
                    if (at < keyFrames.Length)
                        goto case PlaybackMode.Normal;
                    else
                    {
                        at -= keyFrames.Length;
                        goto case PlaybackMode.Reversed;
                    }
            }
            Log.Error("Not yet implemented case :" + playbackMode);
            return default;
        }
    }
}
