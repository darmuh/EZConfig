using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

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
        MenuAPI.AddTab("Mods");

        AddModSettings();

        MenuAPI.AddElementToPauseMenu(x =>
            MenuAPI.CreatePauseMenuButton("Test")?
            .SetColor(Color.cyan)
            .SetBorderColor(Color.red)
            .SetParent(x)
        );
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
            MenuAPI.AddTextToTab(modName, "Mods");
            foreach (var configEntry in configEntryBases)
            {
                try
                {
                    if (configEntry.SettingType == typeof(bool))
                    {
                        var defaultValue = configEntry.DefaultValue is bool dValue && dValue;
                        var currentValue = configEntry.BoxedValue is bool bValue && bValue;

                        MenuAPI.AddBoolToTab(configEntry.Definition.Key, defaultValue, "Mods", currentValue, newVal => configEntry.BoxedValue = newVal);
                    }
                    else if (configEntry.SettingType == typeof(float))
                    {
                        var defaultValue = configEntry.DefaultValue is float cValue ? cValue : 0f;
                        var currentValue = configEntry.BoxedValue is float bValue ? bValue : 0f;

                        MenuAPI.AddFloatToTab(configEntry.Definition.Key, defaultValue, "Mods", 0f, 1000f, currentValue, newVal => configEntry.BoxedValue = newVal);
                    }
                    else if (configEntry.SettingType == typeof(int))
                    {
                        var defaultValue = configEntry.DefaultValue is int cValue ? cValue : 0;
                        var currentValue = configEntry.BoxedValue is int bValue ? bValue : 0;
                        MenuAPI.AddIntToTab(configEntry.Definition.Key, defaultValue, "Mods", currentValue, newVal => configEntry.BoxedValue = newVal);
                    }
                    else if (configEntry.SettingType == typeof(string))
                    {
                        var defaultValue = configEntry.DefaultValue is string cValue ? cValue : "";
                        var currentValue = configEntry.BoxedValue is string bValue ? bValue : "";
                        MenuAPI.AddStringToTab(configEntry.Definition.Key, defaultValue, "Mods", currentValue, newVal => configEntry.BoxedValue = newVal);
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
