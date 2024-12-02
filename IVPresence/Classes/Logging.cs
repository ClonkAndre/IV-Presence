using IVSDKDotNet;

namespace IVPresence.Classes
{
    internal class Logging
    {

        public static void Log(string str, params object[] args)
        {
            IVGame.Console.Print(string.Format("[IVPresence] {0}", string.Format(str, args)));
        }
        public static void LogWarning(string str, params object[] args)
        {
            IVGame.Console.PrintWarning(string.Format("[IVPresence] {0}", string.Format(str, args)));
        }
        public static void LogError(string str, params object[] args)
        {
            IVGame.Console.PrintError(string.Format("[IVPresence] {0}", string.Format(str, args)));
        }

        public static void LogDebug(string str, params object[] args)
        {
#if DEBUG
            IVGame.Console.Print(string.Format("[IVPresence] [DEBUG] {0}", string.Format(str, args)));
#endif
        }

    }
}
