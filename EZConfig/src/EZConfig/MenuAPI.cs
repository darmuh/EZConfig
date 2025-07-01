using EZConfig.SettingOptions;
using EZConfig.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EZConfig
{
    public static class MenuAPI
    {
        internal static BuilderDelegate? pauseMenuBuilderDelegate;
        public delegate void BuilderDelegate(Transform parent);

        public static void AddElementToPauseMenu(BuilderDelegate builderDelegate) => pauseMenuBuilderDelegate += builderDelegate;


        // Tabs 
        #region Tabs
        internal static List<string> CustomTabs { get; private set; } = [];

        public static void AddTab(string tabName)
        {
            if (!CustomTabs.Contains(tabName))
                CustomTabs.Add(tabName);
        }

        public static void AddTextToTab(string displayName, string tabName)
        {
            if (SettingsHandler.Instance == null)
            {
                Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
                return;
            }

            SettingsHandler.Instance.AddSetting(new BepInExHeader(displayName, tabName));
        }

        public static void AddBoolToTab(string displayName, bool defaultValue, string tabName, bool currentValue = false, Action<bool>? saveCallback = null)
        {
            if (SettingsHandler.Instance == null)
            {
                Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
                return;
            }

            SettingsHandler.Instance.AddSetting(new BepInExOffOn(displayName, defaultValue, tabName, currentValue, saveCallback));
        }

        public static void AddFloatToTab(string displayName, float defaultValue,
            string tabName, float minValue = 0f, float maxValue = 1f, float currentValue = 0f, Action<float>? applyCallback = null)
        {
            if (SettingsHandler.Instance == null)
            {
                Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
                return;
            }

            SettingsHandler.Instance.AddSetting(new BepInExFloat(displayName, defaultValue, tabName, minValue, maxValue, currentValue, applyCallback));
        }

        public static void AddIntToTab(string displayName, int defaultValue, string tabName, int currentValue = 0, Action<int>? saveCallback = null)
        {
            if (SettingsHandler.Instance == null)
            {
                Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
                return;
            }

            SettingsHandler.Instance.AddSetting(new BepInExInt(displayName, tabName, defaultValue, currentValue, saveCallback));
        }
        #endregion


        internal static GameObject? pauseMenuButtonTemplate = null;
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
