using EZConfig.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Zorro.Settings;
using static EZConfig.Patching;
using Object = UnityEngine.Object;

namespace EZConfig;

public class Extensions
{
    //public static SharedSettingsMenu SharedSettings = null!;
    public static GameObject ModMenuPrefab = null!;
    public static GameObject ControlsTab = null!;
    public static GameObject StringPrefab = null!;
    public static GameObject IntPrefab = null!;
    public static GameObject ButtonPrefab = null!;
    //public static GameObject SelectedGraphic = null!;
    public static GameObject ModSettingsPage = null!;
    internal static ModMenuWindow SharedModMenu = null!;

    public static void ShowSettingsExtended(SharedSettingsMenu sharedSettings, string categoryName)
    {
        Plugin.Spam($"Selecting category: {categoryName}");
        if (sharedSettings.m_fadeInCoroutine != null)
        {
            sharedSettings.StopCoroutine(sharedSettings.m_fadeInCoroutine);
            sharedSettings.m_fadeInCoroutine = null;
        }

        foreach (SettingsUICell spawnedCell in sharedSettings.m_spawnedCells)
        {
            Object.Destroy(spawnedCell.gameObject);
        }

        sharedSettings.m_spawnedCells.Clear();
        sharedSettings.RefreshSettings();
        foreach (IExposedSetting item in from setting in sharedSettings.settings
                                         where setting.GetCategory() == categoryName
                                         where !(setting is IConditionalSetting conditionalSetting) || conditionalSetting.ShouldShow()
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

        if (AddMenusPatch.scroller == null)
            return;

        AddMenusPatch.scroller.normalizedPosition = Vector3.zero;
    }

    //public static SettingsTABSExtended ExtendedTabs { get; internal set; }
    //public static List<SettingsTABSButtonExtended> ExtendedTabButtons { get; internal set; }

    /*
     * Category Names:
        General,
        Graphics,
        Audio,
        Controls,
        Mods
     */

    /*
    public static void AddExtendedTabs(SharedSettingsMenu sharedSettings)
    {
        Plugin.Spam("Adding extended tabs!");
        ExtendedTabButtons = [];
        ExtendedTabs = sharedSettings.gameObject.AddComponent<SettingsTABSExtended>();
        sharedSettings.m_tabs.buttons.Do(b =>
        {
            SettingsTABSButtonExtended copy = b.gameObject.AddComponent<SettingsTABSButtonExtended>();
            copy.vanillaButton = b;

            copy.UpdateCategoryName();
            ExtendedTabs.AddButton(copy);
            ExtendedTabButtons.Add(copy);
        });
    }

    

    public class SettingsTABSExtended : TABS<SettingsTABSButtonExtended>
    {
        public override void OnSelected(SettingsTABSButtonExtended button)
        {
            ShowSettingsExtended(SharedSettings, button.category);
        }
    }

    public class SettingsTABSButtonExtended : TAB_Button
    {
        public string category;

        public GameObject SelectedGraphic;

        public SettingsTABSButton vanillaButton;

        public void UpdateCategoryName()
        {
            if(gameObject.name != category)
            {
                category = gameObject.name;
                enabled = true;
            }
        }

        public void Update()
        {
            vanillaButton?.Update();

            if (SelectedGraphic == null || vanillaButton != null)
                return;

            Color b = (base.Selected ? Color.black : Color.white);
            text.color = Color.Lerp(text.color, b, Time.unscaledDeltaTime * 7f);
            SelectedGraphic.gameObject.SetActive(base.Selected);
        }
    }*/
}
