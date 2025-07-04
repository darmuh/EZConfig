using System;
using TMPro;
using UnityEngine;
using Zorro.Core;
using Zorro.Settings;
using Zorro.Settings.UI;
using Object = UnityEngine.Object;

namespace EZConfig.SettingOptions;

public class BepInExInt(string displayName, string category, int defaultValue = 0, int currentValue = 0,
    Action<int>? saveCallback = null,
    Action<BepInExInt>? onApply = null) : IntSetting, IExposedSetting
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
                var newIntSetting = _settingUICell.AddComponent<IntSettingUI>();
                newIntSetting.inputField = oldFloatSetting.inputField;

                Object.DestroyImmediate(oldFloatSetting.slider.gameObject);
                Object.DestroyImmediate(oldFloatSetting);

                newIntSetting.inputField.characterValidation = TMP_InputField.CharacterValidation.Integer;
                var inputRectTransform = newIntSetting.inputField.GetComponent<RectTransform>();
                inputRectTransform.pivot = new Vector2(0.5f, 0.5f);
                inputRectTransform.offsetMin = new Vector2(20, -25);
                inputRectTransform.offsetMax = new Vector2(380, 25);

                Object.DontDestroyOnLoad(_settingUICell);
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

    protected override int GetDefaultValue() => defaultValue;
}

public class IntSettingUI : SettingInputUICell
{
    public TMP_InputField inputField;

    public override void Setup(Setting setting, ISettingHandler settingHandler)
    {
        if (setting == null || setting is not BepInExInt intSetting) return;

        RegisterSettingListener(setting);
        inputField.SetTextWithoutNotify(intSetting.Expose(intSetting.Value));
        inputField.onValueChanged.AddListener(OnChanged);

        void OnChanged(string str)
        {
            Plugin.WARNING("New int input: " + str);
            if (int.TryParse(str, out var result))
            {
                inputField.SetTextWithoutNotify(intSetting.Expose(result));
                intSetting.SetValue(result, settingHandler);
            }
        }
    }
    
    protected override void OnSettingChangedExternal(Setting setting)
    {
        base.OnSettingChangedExternal(setting);

        if (setting is BepInExInt intSetting)
            inputField.SetTextWithoutNotify(intSetting.Expose(intSetting.Value));
        
    }
}
