using MelonLoader;

namespace MultiActions
{
    internal static class MultiActionSettings
    {
        internal static string ModName = "MultiActions";
        internal static string ModVersion = "1.0.3";
        internal static MelonPreferences_Entry<bool> enable;
        internal static MelonPreferences_Entry<bool> quitButton;
        internal static MelonPreferences_Entry<bool> respawnButton;

        /// <summary>
        /// Enable risky functions
        /// </summary>
        internal static MelonPreferences_Entry<bool> riskyF;

        public static void RegisterSettings()
        {
            var category = MelonPreferences.CreateCategory(ModName, ModName);
            
            enable = category.CreateEntry("Enable", true, "Enable mod");
            quitButton = category.CreateEntry("Quit Button", false, "Enable quit button");
            respawnButton = category.CreateEntry("Respawn Button", false, "Enable respawn button");
            riskyF = category.CreateEntry("Risky Functions", false, "Enable risky functions");
        }

        public static bool IsModEnabled()
        {
            return enable.Value;
        }
    }
}
