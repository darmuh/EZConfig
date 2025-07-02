using Zorro.Settings;

namespace EZConfig.SettingOptions;

public class BepInExString(string displayName, string defaultValue = "", string categoryName = "Mods") : StringSetting, IExposedSetting
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


    protected override string GetDefaultValue()
    {
        return defaultValue;
    }
}