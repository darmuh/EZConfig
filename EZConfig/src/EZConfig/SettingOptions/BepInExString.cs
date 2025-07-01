﻿using System;
using TMPro;
using UnityEngine;
using Zorro.Core;
using Zorro.Settings;
using Zorro.Settings.UI;
using Object = UnityEngine.Object;

namespace EZConfig.SettingOptions;

public class BepInExString(string displayName, string category, string defaultValue = "", string currentValue = "",
    Action<string>? saveCallback = null,
    Action<BepInExString>? onApply = null) : StringSetting, IExposedSetting
{
    private static GameObject? _settingUICell = null;
    public static GameObject? SettingUICell
    {
        get
        {
            if (_settingUICell == null)
            {
                if (SingletonAsset<InputCellMapper>.Instance == null || SingletonAsset<InputCellMapper>.Instance.FloatSettingCell == null)
                    return null;    

                _settingUICell = Object.Instantiate(SingletonAsset<InputCellMapper>.Instance.FloatSettingCell);
                _settingUICell.name = "BepInExIntCell";

                var oldFloatSetting = _settingUICell.GetComponent<FloatSettingUI>();
                var newStringSetting = _settingUICell.AddComponent<StringSettingUI>();
                newStringSetting.inputField = oldFloatSetting.inputField;

                Object.DestroyImmediate(oldFloatSetting.slider.gameObject);
                Object.DestroyImmediate(oldFloatSetting);

                newStringSetting.inputField.characterValidation = TMP_InputField.CharacterValidation.None;
                var inputRectTransform = newStringSetting.inputField.GetComponent<RectTransform>();
                inputRectTransform.pivot = new Vector2(0.5f, 0.5f);
                inputRectTransform.offsetMin = new Vector2(20, -25);
                inputRectTransform.offsetMax = new Vector2(380, 25);

                var text = newStringSetting.inputField.GetComponentInChildren<TextMeshProUGUI>();
                text.fontSize = text.fontSizeMin = text.fontSizeMax = 22;
                text.alignment = TextAlignmentOptions.MidlineLeft;
            }

            return _settingUICell;
        }
    }

    public override void Load(ISettingsSaveLoad loader)
    {
        Value = currentValue;
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        saveCallback?.Invoke(Value);
    }

    public override void ApplyValue()
    {
        onApply?.Invoke(this);
    }

    public override GameObject? GetSettingUICell() => SettingUICell;

    public string GetCategory() => category;

    public string GetDisplayName() => displayName;

    protected override string GetDefaultValue() => defaultValue;
}

public class StringSettingUI : SettingInputUICell
{
    public TMP_InputField inputField;

    public override void Setup(Setting setting, ISettingHandler settingHandler)
    {
        if (setting == null || setting is not BepInExString stringSetting) return;

        RegisterSettingListener(setting);
        inputField.SetTextWithoutNotify(stringSetting.Value);
        inputField.onValueChanged.AddListener(OnChanged);

        void OnChanged(string str)
        {
            inputField.SetTextWithoutNotify(str);
            stringSetting.SetValue(str, settingHandler);
        }
    }
    
    protected override void OnSettingChangedExternal(Setting setting)
    {
        base.OnSettingChangedExternal(setting);

        if (setting is BepInExString stringSetting)
            inputField.SetTextWithoutNotify(stringSetting.Value);
        
    }
}
