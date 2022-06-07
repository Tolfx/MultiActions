using MelonLoader;

namespace MultiActions
{
    internal static class MultiActionSettings
    {
        internal static string ModName = "MultiActions";
        internal static string ModVersion = "1.0.1";
        internal static MelonPreferences_Entry<bool> enable;
        internal static MelonPreferences_Entry<bool> quitButton;
        internal static MelonPreferences_Entry<bool> respawnButton;

        public static void RegisterSettings()
        {
            var category = MelonPreferences.CreateCategory(ModName, ModName);
            
            enable = category.CreateEntry("Enable", true, "Enable mod");
            quitButton = category.CreateEntry("Quit Button", false, "Enable quit button");
            respawnButton = category.CreateEntry("Respawn Button", false, "Enable respawn button");
        }

        public static bool IsModEnabled()
        {
            return enable.Value;
        }
    }
}
