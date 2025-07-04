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
        internal static BuilderDelegate? pauseMenuBuilderDelegate, mainMenuBuilderDelegate;
        public delegate void BuilderDelegate(Transform parent);

        public static void AddElementToMainMenu(BuilderDelegate builderDelegate) => mainMenuBuilderDelegate += builderDelegate;
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

        public static void AddStringToTab(string displayName, string defaultValue, string tabName, string currentValue = "", Action<string>? saveCallback = null)
        {
            if (SettingsHandler.Instance == null)
            {
                Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
                return;
            }

            SettingsHandler.Instance.AddSetting(new BepInExString(displayName, tabName, defaultValue, currentValue, saveCallback));
        }


        public static void AddKeybindToTab(string displayName, KeyCode defaultValue, string tabName, KeyCode currentValue, Action<KeyCode>? saveCallback)
        {
            if (SettingsHandler.Instance == null)
            {
                Plugin.Log.LogError("You're registering options too early! Use the Start() function to create new options!");
                return;
            }

            SettingsHandler.Instance.AddSetting(new BepInExKeyCode(displayName, tabName, defaultValue, currentValue, saveCallback));
        }
        #endregion


        public static CustomPage CreatePage(string pageName)
        {
            var page = new GameObject(pageName, typeof(CustomPage));

            return page.GetComponent<CustomPage>();
        }

        public static PeakMenuButton? CreateMenuButton(string buttonName)
        {
            if (PeakTemplates.ButtonTemplate == null)
            {
                Plugin.ERROR("You're creating PauseMenuButton too early! Prefab hasn't been loaded yet.");

                return null;
            }

            var clone = Object.Instantiate(PeakTemplates.ButtonTemplate);
            clone.name = $"UI_MainMenuButton_{buttonName}";

            var newButton = clone.AddComponent<PeakMenuButton>();

            return newButton.SetText(buttonName);

        }

        public static PeakText CreateText(string displayText, string objectName = "UI_PeakText")
        {
            var gameObj = new GameObject(objectName, typeof(PeakText));

            return gameObj.GetComponent<PeakText>().SetText(displayText);
        }

        public const float OPTIONS_WIDTH = 277f;
        public static PeakMenuButton? CreatePauseMenuButton(string buttonName) => CreateMenuButton(buttonName)?.SetWidth(OPTIONS_WIDTH);
        
    }
}
