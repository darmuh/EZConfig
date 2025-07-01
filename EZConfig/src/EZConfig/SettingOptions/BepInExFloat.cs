using System;
using Unity.Mathematics;
using Zorro.Settings;

namespace EZConfig.SettingOptions;

public class BepInExFloat(string displayName, float defaultValue = 0f, string categoryName = "Mods", 
    float minValue = 0f, float maxValue = 1f, float currentValue = 0f, 
    Action<float>? saveCallback = null,
    Action<BepInExFloat>? onApply = null) : FloatSetting, IExposedSetting
{
    public override void Load(ISettingsSaveLoad loader)
    {
        Value = currentValue;

        float2 minMaxValue = GetMinMaxValue();
        MinValue = minMaxValue.x;
        MaxValue = minMaxValue.y;
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        Plugin.Log.LogWarning("Saved called");
        saveCallback?.Invoke(Value);
        //base.Save(saver);
    }

    public override void ApplyValue()
    {
        onApply?.Invoke(this);
    }

    public string GetDisplayName() => displayName;

    public string GetCategory() => categoryName;


    protected override float GetDefaultValue()
    {
        return defaultValue;
    }

    protected override float2 GetMinMaxValue()
    {
        return new float2(minValue, maxValue);
    }
}

