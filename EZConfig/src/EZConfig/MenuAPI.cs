using EZConfig.SettingOptions;
using EZConfig.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EZConfig
{
    public static class MenuAPI
    {
        internal static BuilderDelegate pauseMenuBuilderDelegate = null!;
        public delegate void BuilderDelegate(Transform parent);

        public static void AddElementToPauseMenu(BuilderDelegate builderDelegate) => pauseMenuBuilderDelegate += builderDelegate;


        // Tabs 
        #region Tabs
        internal static List<string> CustomTabs { get; private set; } = [];
        internal static Dictionary<string, List<string>> ModMenus { get; private set; } = [];

        public static void AddSettingsTab(string tabName)
        {
            if (!CustomTabs.Contains(tabName))
                CustomTabs.Add(tabName);
        }

        public static void AddModTab(string modName, string tabName)
        {
            if (!ModMenus.ContainsKey(modName))
            {
                ModMenus.Add(modName, [tabName]);
                SettingsHandler.Instance.AddSetting(new ModSettingButton(modName));
                return;
            }

            if (!ModMenus[modName].Contains(tabName))
                ModMenus[modName].Add(tabName);
            
        }

        public static void AddBoolToTab(string displayName, bool defaultValue, string tabName)
        {
            if (SettingsHandler.Instance == null)
            {
                Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
                return;
            }

            SettingsHandler.Instance.AddSetting(new BepInExOffOn(displayName, defaultValue, tabName));
        }

        public static void AddFloatToTab(string displayName, float defaultValue,
            string tabName, float minValue = 0f, float maxValue = 1f)
        {
            if (SettingsHandler.Instance == null)
            {
                Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
                return;
            }

            SettingsHandler.Instance.AddSetting(new BepInExFloat(displayName, defaultValue, tabName, minValue, maxValue));
        }

        public static void AddIntToTab(string displayName, int defaultValue,
            string tabName, int minValue = 0, int maxValue = 1)
        {
            if (SettingsHandler.Instance == null)
            {
                Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
                return;
            }

            SettingsHandler.Instance.AddSetting(new BepInExInt(displayName, defaultValue, tabName, minValue, maxValue));
        }

        public static void AddStringToTab(string displayName, string defaultValue, string tabName)
        {
            if (SettingsHandler.Instance == null)
            {
                Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
                return;
            }

            SettingsHandler.Instance.AddSetting(new BepInExString(displayName, defaultValue, "General"));
        }
        #endregion

        internal static GameObject? pauseMenuButtonTemplate = null;

        public static void CreateModPageHandler()
        {
            var main = GameObject.Find("MainMenu");

            if (main != null)
            {
                main.AddComponent<ModPageHandler>();
                Plugin.Spam("ModPageHandler created!");
            }
        }

        public static PauseMenuButton? CreatePauseMenuButton(string buttonName)
        {
            if (pauseMenuButtonTemplate == null)
            {
                var game = GameObject.Find("GAME");
                if (game != null)
                    pauseMenuButtonTemplate = Object.Instantiate(game.transform.Find("GUIManager/PauseCanvases/Canvas_Options/MainPage/Options/UI_MainMenuButton_Resume").gameObject);

            }

            if (pauseMenuButtonTemplate != null)
            {
                var clone = Object.Instantiate(pauseMenuButtonTemplate);
                clone.name = $"UI_MainMenuButton_{buttonName}";

                var newButton = clone.AddComponent<PauseMenuButton>();

                newButton.SetText(buttonName);
                return newButton;
            }

            return null;
        }
    }
}
