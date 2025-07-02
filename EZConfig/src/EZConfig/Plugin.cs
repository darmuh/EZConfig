using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using EZConfig.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Settings;

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
        LoadCustomPrefabs();
        AddModSettings();

        MenuAPI.AddElementToPauseMenu(x =>
        {
            var button = MenuAPI.CreatePauseMenuButton("Test");
            if (button != null)
            {
                button.SetColor(Color.cyan);
                button.SetBorderColor(Color.red);
                button.transform.SetParent(x);
            }
        });
    }

    internal static void LoadCustomPrefabs()
    {
        string assetBundleFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ezconfig");
        AssetBundle bundle = AssetBundle.LoadFromFile(assetBundleFilePath);
        //ModMenu
        Extensions.ModMenuPrefab = bundle.LoadAsset<GameObject>("ModMenu.prefab");
        Extensions.StringPrefab = bundle.LoadAsset<GameObject>("STRING INPUT.prefab");
        Extensions.IntPrefab = bundle.LoadAsset<GameObject>("INT INPUT.prefab");
        Extensions.ButtonPrefab = bundle.LoadAsset<GameObject>("BUTTON INPUT.prefab");
        AssignPrefabs();
    }

    internal static void AssignPrefabs()
    {
        if (Extensions.IntPrefab == null || Extensions.StringPrefab == null || Extensions.ButtonPrefab == null)
        {
            WARNING("Unable to map custom prefabs to Zorro's InputCellMapper!");
            return;
        }

        InputCellMapper.Instance.StringSettingCell = Extensions.StringPrefab;
        InputCellMapper.Instance.IntSettingCell = Extensions.IntPrefab;
        InputCellMapper.Instance.ButtonSettingCell = Extensions.ButtonPrefab;
        Spam("Prefabs loaded into InputCellMapper!");
        FixPrefabs(InputCellMapper.Instance.StringSettingCell);
        FixPrefabs(InputCellMapper.Instance.IntSettingCell);
        //FixPrefabs(InputCellMapper.Instance.ButtonSettingCell);
    }

    internal static void FixPrefabs(GameObject broken)
    {
        if (InputCellMapper.Instance.FloatSettingCell == null)
        {
            ERROR("FixPrefabs called too early!!!");
            return;
        }
        
        if(broken == null)
        {
            WARNING("Calling fix on null object!"); 
            return;
        }

        broken.GetComponentsInChildren<TextMeshProUGUI>().DoIf(t => t != null, t => t.font = InputCellMapper.Instance.FloatSettingCell.GetComponentInChildren<TextMeshProUGUI>().font);

        Spam($"Fixed prefab {broken.name}");
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

    internal static void MESSAGE(string message)
    {
        Log.LogMessage(message);
    }

    private static bool modSettingsAdded = false;
    private static void AddModSettings()
    {
        if (modSettingsAdded) return;

        modSettingsAdded = true;
        MenuAPI.AddSettingsTab("Mods");

        foreach (var (modName, configEntryBases) in GetModConfigEntries())
        {  
            foreach (var configEntry in configEntryBases)
            {
                try
                {
                    MenuAPI.AddModTab(modName, configEntry.Definition.Section);

                    if (configEntry.SettingType == typeof(bool))
                    {
                        var defaultValue = configEntry.DefaultValue is bool value && value;
                        MenuAPI.AddBoolToTab(configEntry.Definition.Key, defaultValue, configEntry.Definition.Section);
                    }
                    else if (configEntry.SettingType == typeof(float))
                    {
                        var defaultValue = configEntry.DefaultValue is float value ? value : 0f;
                        MenuAPI.AddFloatToTab($"{configEntry.Definition.Key}", defaultValue, configEntry.Definition.Section);
                    }
                    else if (configEntry.SettingType == typeof(int))
                    {
                        var defaultValue = configEntry.DefaultValue is int value ? value : 0;
                        MenuAPI.AddIntToTab($"{configEntry.Definition.Key}", defaultValue, configEntry.Definition.Section);
                    }
                    else if (configEntry.SettingType == typeof(string))
                    {
                        var defaultValue = configEntry.DefaultValue is string value ? value : "";
                        MenuAPI.AddStringToTab($"{configEntry.Definition.Key}", defaultValue, configEntry.Definition.Section);
                    }
                    else // Warn about missing SettingTypes
                        WARNING($"Unable to add setting from {modName}: {configEntry.Definition.Key} - {configEntry.SettingType}");
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
