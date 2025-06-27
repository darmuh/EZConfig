using System.Collections.Generic;
using UnityEngine.Localization;
using Zorro.Settings;

namespace EZConfig.SettingOptions;

public class BepInExOffOn(string displayName, bool defaultValue = false, string category = "Mods") : OffOnSetting, IExposedSetting
{
    public override void Load(ISettingsSaveLoad loader)
    {
        Value = OffOnMode.OFF;
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        //base.Save(saver);
    }

    public override void ApplyValue()
    {
        // Apply to BepInEx
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
