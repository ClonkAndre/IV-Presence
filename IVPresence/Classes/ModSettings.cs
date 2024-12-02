using IVSDKDotNet;

namespace IVPresence.Classes
{
    internal class ModSettings
    {

        #region Variables
        // General
        public static bool LogErrorsToConsole;
        public static bool AllowOtherModsToSetPresence;
        public static bool ShowPresenceOfOtherMods;

        // Credits
        public static bool ShowStatistics;

        // Network
        public static bool ShowNetworkKillerName;
        #endregion

        public static void Load(SettingsFile settingsFile)
        {
            // General
            LogErrorsToConsole = settingsFile.GetBoolean("General", "LogErrorsToConsole", true);
            AllowOtherModsToSetPresence = settingsFile.GetBoolean("General", "AllowOtherModsToSetPresence", true);
            ShowPresenceOfOtherMods = settingsFile.GetBoolean("General", "ShowPresenceOfOtherMods", true);

            // Credits
            ShowStatistics = settingsFile.GetBoolean("Credits", "ShowStatistics", true);

            // Network
            ShowNetworkKillerName = settingsFile.GetBoolean("Network", "ShowNetworkKillerName", true);
        }

    }
}
