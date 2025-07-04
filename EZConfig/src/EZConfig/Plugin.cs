using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using EZConfig.Components;
using EZConfig.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EZConfig;

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

        Patching.Initialize();

        Spam("Load complete!");
    }

    private void Start()
    {
        // Create entries for all loaded mods
        AddModSettings();

        void builderDelegate(Transform parent)
        {
            var modSettingsPage = MenuAPI.CreatePage("ModSettings").CreateBackground();

            var newText = MenuAPI.CreateText("Mod Settings", "Header")
                .SetFontSize(48)
                .ParentTo(modSettingsPage.transform)
                .SetPosition(new Vector2(100f, -60f));

            var backButton = MenuAPI.CreateMenuButton("Back")?
                .SetColor(new Color(1, 0.5f, 0.2f))
                .ParentTo(modSettingsPage.transform)
                .SetPosition(new Vector2(230, -160))
                .SetWidth(200)
                .OnClick(modSettingsPage.Close);

            var content = new GameObject("Content")
                .AddComponent<PeakElement>()
                .ParentTo(modSettingsPage.transform)
                .SetPivot(new Vector2(0, 1))
                .SetAnchorMin(new Vector2(0, 1))
                .SetAnchorMax(new Vector2(0, 1))
                .SetPosition(new Vector2(428, -70))
                .SetSize(new Vector2(1360, 980)).RectTransform;

            var settingsMenu = content.gameObject.AddComponent<ModdedSettingsMenu>();

            var horizontalTabs = new GameObject("TABS")
                .ParentTo(content.transform)
                .AddComponent<PeakHorizontalTabs>();

            var moddedSettingsTABS = horizontalTabs.gameObject.AddComponent<ModdedSettingsTABS>();
            moddedSettingsTABS.SettingsMenu = settingsMenu;
            
            var tabContent = new GameObject("TabContent").ParentTo(content.transform).AddComponent<PeakTabContent>();

            settingsMenu.ContentParent = tabContent.Content;
            settingsMenu.Tabs = moddedSettingsTABS;

            foreach (var (modName, configEntryBases) in GetModConfigEntries())
                horizontalTabs.AddTab(modName);

            var isTitleScreen = SceneManager.GetActiveScene().name == "Title";
            
            var pauseOptionsMenu = FindAnyObjectByType<PauseOptionsMenu>();
            var modSettingsButton = MenuAPI.CreatePauseMenuButton("Mod Settings")?
                .SetColor(new Color(0.15f, 0.75f, 0.85f))
                .ParentTo(parent)
                .OnClick(() =>
                {
                    UIInputHandler.SetSelectedObject(null);
                    if (!isTitleScreen) pauseOptionsMenu?.Close();
                    modSettingsPage.Open();
                });

            if (modSettingsButton != null && isTitleScreen)
                modSettingsButton.SetPosition(new Vector2(-140, -210)).SetWidth(200);
            
        }

        MenuAPI.AddElementToPauseMenu(builderDelegate);
        MenuAPI.AddElementToMainMenu(builderDelegate);
    }

    internal static void Spam(string message)
    {
        //if (ModConfig.DeveloperLogging.Value)
        Log.LogDebug(message);
        //else
    }

    internal static void ERROR(string message)
    {
        Log.LogError(message);
    }

    internal static void WARNING(string message)
    {
        Log.LogWarning(message);
    }

    private static bool modSettingsAdded = false;
    private static void AddModSettings()
    {
        if (modSettingsAdded) return;

        modSettingsAdded = true;

        foreach (var (modName, configEntryBases) in GetModConfigEntries())
        {
            foreach (var configEntry in configEntryBases)
            {
                try
                {
                    if (configEntry.SettingType == typeof(bool))
                    {
                        var defaultValue = configEntry.DefaultValue is bool dValue && dValue;
                        var currentValue = configEntry.BoxedValue is bool bValue && bValue;

                        MenuAPI.AddBoolToTab(configEntry.Definition.Key, defaultValue, modName, currentValue, newVal => configEntry.BoxedValue = newVal);
                    }
                    else if (configEntry.SettingType == typeof(float))
                    {
                        var defaultValue = configEntry.DefaultValue is float cValue ? cValue : 0f;
                        var currentValue = configEntry.BoxedValue is float bValue ? bValue : 0f;
 
                        float minValue = 0f;
                        float maxValue = 1000f;

                        if (configEntry.Description.AcceptableValues is AcceptableValueRange<float> range)
                        {
                            minValue = range.MinValue;
                            maxValue = range.MaxValue;
                        }

                        MenuAPI.AddFloatToTab(configEntry.Definition.Key, defaultValue, modName, minValue, maxValue, currentValue, newVal => configEntry.BoxedValue = newVal);
                    }

                    else if (configEntry.SettingType == typeof(int))
                    {
                        var defaultValue = configEntry.DefaultValue is int cValue ? cValue : 0;
                        var currentValue = configEntry.BoxedValue is int bValue ? bValue : 0;
                        MenuAPI.AddIntToTab(configEntry.Definition.Key, defaultValue, modName, currentValue, newVal => configEntry.BoxedValue = newVal);
                    }
                    else if (configEntry.SettingType == typeof(string))
                    {
                        var defaultValue = configEntry.DefaultValue is string cValue ? cValue : "";
                        var currentValue = configEntry.BoxedValue is string bValue ? bValue : "";
                        MenuAPI.AddStringToTab(configEntry.Definition.Key, defaultValue, modName, currentValue, newVal => configEntry.BoxedValue = newVal);
                    }
                    else if (configEntry.SettingType == typeof(KeyCode))
                    {
                        var defaultValue = configEntry.DefaultValue is KeyCode cValue ? cValue : KeyCode.None;
                        var currentValue = configEntry.BoxedValue is KeyCode bValue ? bValue : KeyCode.None;

                        MenuAPI.AddKeybindToTab(configEntry.Definition.Key, defaultValue, modName, currentValue, newVal => configEntry.BoxedValue = newVal);
                    }
                    else // Warn about missing SettingTypes
                        WARNING($"{modName} - {configEntry.Definition.Key} - {configEntry.SettingType}");
                }

                catch (Exception e)
                {
                    Log.LogError(e);
                }
            }

        }
    }

    // From https://github.com/IsThatTheRealNick/REPOConfig/blob/main/REPOConfig/ConfigMenu.cs#L453
    private static Dictionary<string, ConfigEntryBase[]> GetModConfigEntries()
    {
        var peakConfigs = new Dictionary<string, ConfigEntryBase[]>();

        foreach (var plugin in Chainloader.PluginInfos.Values.OrderBy(p => p.Metadata.Name))
        {
            var configEntries = new List<ConfigEntryBase>();

            foreach (var configEntryBase in plugin.Instance.Config.Select(configEntry => configEntry.Value))
            {
                var tags = configEntryBase.Description?.Tags;

                if (tags != null && tags.Contains("Hidden")) continue;

                configEntries.Add(configEntryBase);
            }

            if (configEntries.Count > 0)
                peakConfigs.TryAdd(FixNaming(plugin.Metadata.Name), [.. configEntries]);
        }

        return peakConfigs;
    }

    private static string FixNaming(string input)
    {
        input = Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
        input = Regex.Replace(input, "([A-Z])([A-Z][a-z])", "$1 $2");
        input = Regex.Replace(input, @"\s+", " ");
        input = Regex.Replace(input, @"([A-Z]\.)\s([A-Z]\.)", "$1$2");

        return input.Trim();
    }
}
