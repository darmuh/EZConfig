using EZConfig.UI;
using HarmonyLib;
using pworld.Scripts.Extensions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Zorro.Settings;
using Zorro.UI;
using Object = UnityEngine.Object;

namespace EZConfig;

internal static class Patching
{
    internal static void Initialize()
    {
        Plugin.MESSAGE("Initializing patches");

        var harmony = new Harmony("ezconfig");
        harmony.PatchAll(typeof(Patching).Assembly);

        Plugin.MESSAGE("Patches applied");
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

    [HarmonyPatch(typeof(MainMenuMainPage), nameof(MainMenuMainPage.Start))]
    public class AddToMainMenu
    {
        public static GameObject ModMenuPage = null!;
        public static void Postfix()
        {
            ModMenuPage = GameObject.Instantiate(Extensions.ModMenuPrefab);
        }
    }

    [HarmonyPatch(typeof(SharedSettingsMenu), nameof(SharedSettingsMenu.RefreshSettings))]
    public class AddMenusPatch
    {
        internal static ScrollRect scroller = null!;
        public static bool Prefix(SharedSettingsMenu __instance)
        {
            //Add scroller component
            if(scroller == null && __instance.transform.parent.gameObject.GetComponent<ScrollRect>() == null)
            {
                __instance.m_settingsContentParent.gameObject.GetComponent<RectTransform>().sizeDelta = new(__instance.m_settingsContentParent.gameObject.GetComponent<RectTransform>().sizeDelta.x, -150f);
                scroller = __instance.transform.parent.gameObject.AddComponent<ScrollRect>();
                scroller.horizontal = false;
                scroller.vertical = true;
                scroller.content = __instance.m_settingsContentParent.gameObject.GetComponent<RectTransform>();
                scroller.viewport = __instance.transform.parent.gameObject.GetComponent<RectTransform>();
                scroller.scrollSensitivity = 5f;
                scroller.verticalNormalizedPosition = 1f;
                //scroller.elasticity = 0.25f;
                scroller.movementType = ScrollRect.MovementType.Unrestricted;
                scroller.SetLayoutVertical();
                scroller.enabled = true;  
            }

            foreach (var tabName in MenuAPI.CustomTabs)
            {
                // Custom settings tabs have been created already
                if (__instance.m_tabs.buttons.Any(x => x.name == tabName))
                    continue;

                if (__instance.m_tabs.buttons.Count > 0)
                {
                    var template = __instance.m_tabs.buttons[0];

                    var customTab = Object.Instantiate(template.gameObject, template.gameObject.transform.parent).GetComponent<SettingsTABSButton>();

                    customTab.name = customTab.text.text = tabName;
                    customTab.text.enableAutoSizing = true;
                    customTab.text.fontSizeMin = 14f;
                    customTab.text.fontSizeMax = 24f;
                    customTab.text.verticalAlignment = TMPro.VerticalAlignmentOptions.Geometry;
                    customTab.gameObject.SetActive(true);
                    __instance.m_tabs.buttons.Add(customTab);
                }
            }

            return true;
        }
    }
}
