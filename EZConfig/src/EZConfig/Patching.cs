using HarmonyLib;
using System.Linq;
using Object = UnityEngine.Object;

namespace EZConfig;

internal static class Patching
{
    internal static void Initialize()
    {
        Plugin.WARNING("Initializing patches");

        var harmony = new Harmony("ezconfig");
        harmony.PatchAll(typeof(Patching).Assembly);

        Plugin.WARNING("Patches applied");
    }

    [HarmonyPatch(typeof(SettingsTABS), "OnSelected")]
    public class TabSelectOverride
    {
        public static bool Prefix(SettingsTABS __instance, SettingsTABSButton button)
        {
            Extensions.ShowSettingsExtended(__instance.SettingsMenu, button.name);
            return false;
        }
    }

    [HarmonyPatch(typeof(PauseOptionsMenu), nameof(PauseOptionsMenu.Initialize))]
    public class PauseOptionsMenu_OnOpen
    {
        public static bool Prefix(PauseOptionsMenu __instance)
        {
            var transform = __instance.transform.Find("MainPage/Options");
            if (transform != null)
                MenuAPI.pauseMenuBuilderDelegate?.Invoke(transform);

            return true;
        }
    }

    //SharedSettingsMenu Awake postfix
    [HarmonyPatch(typeof(SharedSettingsMenu), nameof(SharedSettingsMenu.RefreshSettings))]
    public class AddMenusPatch
    {
        public static bool Prefix(SharedSettingsMenu __instance)
        {
            // Mods tab have been created already
            foreach(var tabName in MenuAPI.CustomTabs)
            {
                if (__instance.m_tabs.buttons.Any(x => x.name == tabName)) continue;

                if (__instance.m_tabs.buttons.Count > 0)
                {
                    var template = __instance.m_tabs.buttons[0];

                    var customTab = Object.Instantiate(template.gameObject, template.gameObject.transform.parent).GetComponent<SettingsTABSButton>();

                    customTab.name = customTab.text.text = tabName;
                    customTab.gameObject.SetActive(true);
                    __instance.m_tabs.buttons.Add(customTab);
                }
            }

            return true;
        }
    }
}
