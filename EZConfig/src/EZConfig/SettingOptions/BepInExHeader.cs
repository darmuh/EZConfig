using UnityEngine;
using Zorro.Settings;
using Zorro.Settings.DebugUI;

namespace EZConfig.SettingOptions;

public class BepInExHeader(string displayName, string category) : Setting, IExposedSetting
{
    private static GameObject? _settingUICell = null;
    public static GameObject SettingUICell
    {
        get
        {
            if (_settingUICell == null)
            {
                _settingUICell = new GameObject("BepInExHeaderCell", typeof(HeaderSettingUI));
                Object.DontDestroyOnLoad(_settingUICell);
            }
            return _settingUICell;
        }
    }

    public string GetDisplayName() => displayName;
    public string GetCategory() => category;
    public override GameObject GetSettingUICell() => SettingUICell;
    public override SettingUI? GetDebugUI(ISettingHandler settingHandler) => null;

    public override void Load(ISettingsSaveLoad loader)
    {
        // nothing
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        // nothing
    }
    public override void ApplyValue()
    {
        // nothing
    }
}


public class HeaderSettingUI : SettingInputUICell
{
    public override void Setup(Setting setting, ISettingHandler settingHandler)
    {
        RegisterSettingListener(setting);
    }
}