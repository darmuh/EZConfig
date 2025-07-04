using Mono.Cecil;
using System.Linq;
using UnityEngine;
using Zorro.Settings;
using Object = UnityEngine.Object;

namespace EZConfig;

public class Extensions
{
    //public static SharedSettingsMenu SharedSettings = null!;
    public static GameObject ControlsTab = null!;
    public static GameObject StringPrefab = null!;

    public static void ShowSettingsExtended(SharedSettingsMenu sharedSettings, string categoryName)
    {
        Plugin.Spam($"Selecting category: {categoryName}");
        if (sharedSettings.m_fadeInCoroutine != null)
        {
            sharedSettings.StopCoroutine(sharedSettings.m_fadeInCoroutine);
            sharedSettings.m_fadeInCoroutine = null;
        }

        foreach (SettingsUICell spawnedCell in sharedSettings.m_spawnedCells)
            Object.Destroy(spawnedCell.gameObject);
        

        sharedSettings.m_spawnedCells.Clear();
        sharedSettings.RefreshSettings();
        foreach (IExposedSetting item in from setting in sharedSettings.settings
                                         where setting.GetCategory() == categoryName
                                         where setting is not IConditionalSetting conditionalSetting || conditionalSetting.ShouldShow()
                                         select setting)
        {
            if (item == null || item is not Setting setting || setting.GetSettingUICell() == null)
            {
                Plugin.Log.LogError($"Error, probably GetSettingUICell() is null");
                continue;
            }

            SettingsUICell component = Object.Instantiate(sharedSettings.m_settingsCellPrefab, sharedSettings.m_settingsContentParent).GetComponent<SettingsUICell>();
            sharedSettings.m_spawnedCells.Add(component);

            component.Setup(setting);
        }

        sharedSettings.m_fadeInCoroutine = sharedSettings.StartCoroutine(sharedSettings.FadeInCells());
    }
}
