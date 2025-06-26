using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace EZConfig
{
    [BepInAutoPlugin]
    public partial class Plugin : BaseUnityPlugin
    {
        public static Plugin instance = null!;
        internal static ManualLogSource Log { get; private set; } = null!;

        private void Awake()
        {
            instance = this;
            Log = Logger;
            Log.LogInfo($"Plugin {Name} is loaded!");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Spam("Load complete!");
        }

        internal static void Spam(string message)
        {
            //if (ModConfig.DeveloperLogging.Value)
            Log.LogDebug(message);
            //else
            return;
        }

        internal static void ERROR(string message)
        {
            Log.LogError(message);
        }

        internal static void WARNING(string message)
        {
            Log.LogWarning(message);
        }
    }
}
