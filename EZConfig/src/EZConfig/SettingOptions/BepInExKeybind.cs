//using System;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;
//using UnityEngine.Localization;
//using Zorro.Settings;

//namespace EZConfig.SettingOptions;

//public class BepInExKeybind(string displayName, string category, KeyCode defaultValue = KeyCode.None, KeyCode currentValue = KeyCode.None,
//    Action<KeyCode>? saveCallback = null,
//    Action<BepInExKeybind>? onApply = null) : EnumSetting<KeyCode>, IExposedSetting
//{
//    public string GetCategory() => category;
//    public string GetDisplayName() => displayName;

//    public override void Load(ISettingsSaveLoad loader)
//    {
//        Value = currentValue;
//    }

//    public override void Save(ISettingsSaveLoad saver)
//    {
//        saveCallback?.Invoke(Value);
//    }

//    public override void ApplyValue()
//    {
//        onApply?.Invoke(this);
//    }

//    public override List<LocalizedString> GetLocalizedChoices()
//    {
//        return null;
//    }

//    protected override KeyCode GetDefaultValue() => defaultValue;
    
//}
