using System;
using System.Diagnostics;

namespace MonoNet.Util
{
    public class Log
    {
        private static Log instance;

        public enum Level
        {
            PrintNothing, PrintMessages, PrintMessagesAndStackTrace
        }

        private Level infoLevel, warnLevel, errorLevel;

        public Log(Level infoLevel, Level warnLevel, Level errorLevel)
        {
            instance = this;

            this.infoLevel = infoLevel;
            this.warnLevel = warnLevel;
            this.errorLevel = errorLevel;
        }

        public static void SetLogLevel(Level level) => instance.infoLevel = level;

        public static void SetErrorLevel(Level level) => instance.errorLevel = level;

        public static void SetWarnLevel(Level level) => instance.warnLevel = level;

        /// <summary>
        /// Used for non error messages.
        /// </summary>
        /// <param name="s">The string that should be printed.</param>
        public static void Info(string s) => Print(s, instance.infoLevel, ConsoleColor.Black);

        /// <summary>
        /// Used for critical errors.
        /// </summary>
        /// <param name="s">The string that should be printed.</param>
        public static void Error(string s) => Print(s, instance.errorLevel, ConsoleColor.Red);

        /// <summary>
        /// Used for recoverable errors.
        /// </summary>
        /// <param name="s">The string that should be printed.</param>
        public static void Warn(string s) => Print(s, instance.warnLevel, ConsoleColor.Cyan);

        private static void Print(string s, Level level, ConsoleColor foregroundColor)
        {
            Console.ForegroundColor = foregroundColor;
            switch (level)
            {
                case Level.PrintMessages:
                    Debug.WriteLine(s);
                    break;
                case Level.PrintMessagesAndStackTrace:
                    Debug.WriteLine(s + "\n" + new StackTrace().ToString());
                    break;
            }
            Console.ResetColor();
        }
    }
}
