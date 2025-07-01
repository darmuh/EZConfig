using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

namespace EZConfig.SettingOptions;

public class BepInExOffOn(string displayName, bool defaultValue = false, string category = "Mods", bool currentValue = false, Action<bool>? saveCallback = null, Action<BepInExOffOn>? onApply = null) : OffOnSetting, IExposedSetting
{
    public override void Load(ISettingsSaveLoad loader)
    {
        Value = currentValue ? OffOnMode.ON : OffOnMode.OFF;
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        saveCallback?.Invoke(Value == OffOnMode.ON);
        //base.Save(saver);
    }

    public override void ApplyValue()
    {
        onApply?.Invoke(this);
    }

    protected override OffOnMode GetDefaultValue()
    {
        return defaultValue == true ? OffOnMode.ON : OffOnMode.OFF;
    }

    public string GetDisplayName() => displayName;

    public string GetCategory() => category;

    public override List<LocalizedString> GetLocalizedChoices()
    {
        return null;
    }
}
