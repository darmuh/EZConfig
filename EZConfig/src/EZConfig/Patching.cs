using HarmonyLib;
using System.Linq;
using UnityEngine;
using Zorro.UI;
using static EZConfig.Extensions;

namespace EZConfig
{

    [HarmonyPatch(typeof(SettingsTABS), "OnSelected")]
    public class TabSelectOverride
    {
        public static bool Prefix(SettingsTABS __instance, SettingsTABSButton button)
        {
            ShowSettingsExtended(__instance.SettingsMenu, button.name);
            return false;
        }
    }

    [HarmonyPatch(typeof(GameHandler), "Awake")]
    public class InitModdedMenus
    {
        public static void Postfix()
        {
            ControlSettings.InitControls();
        }
    }

    //SharedSettingsMenu Awake postfix
    [HarmonyPatch(typeof(SharedSettingsMenu), "OnEnable")]
    public class AddMenusPatch
    {
        public static void Prefix(SharedSettingsMenu __instance)
        {
            Plugin.Spam("Enabling disabled tabs!!!");
            __instance.m_tabs.buttons.DoIf(b => !b.gameObject.activeSelf, b => 
            {
                b.gameObject.SetActive(true);
                ControlsTab = b.gameObject;
                GameObject Mods = GameObject.Instantiate(b.gameObject, b.gameObject.transform.parent);
                Mods.name = "Mods";
                Mods.GetComponent<SettingsTABSButton>().text.text = "Mods";
            });
        }
    }
}
