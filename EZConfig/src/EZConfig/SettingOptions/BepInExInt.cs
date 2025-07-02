using Unity.Mathematics;
using Zorro.Settings;

namespace EZConfig.SettingOptions;

public class BepInExInt(string displayName, int defaultValue = 0, string categoryName = "Mods", int minValue = 0, int maxValue = 1) : FloatSetting, IExposedSetting
{
    public override void Load(ISettingsSaveLoad loader)
    {
        //base.Load(loader);
    }
    public override void Save(ISettingsSaveLoad saver)
    {
        //base.Save(saver);
    }

    public override void ApplyValue()
    {
        // Apply to BepInEx
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