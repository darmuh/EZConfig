using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zorro.Settings;
using Zorro.UI;
using Object = UnityEngine.Object;
using System.Linq;
using EZConfig.UI;

namespace EZConfig.Components;
#nullable disable
public class ModdedSettingsMenu : MonoBehaviour
{
    private void OnEnable()
    {
        RefreshSettings();

        if (Tabs != null && Tabs.selectedButton != null)
            Tabs.Select(Tabs.selectedButton);
        
    }

    public ModdedSettingsTABS Tabs { get; set; }
    public Transform ContentParent { get; set; }

    private List<IExposedSetting> settings;

    private readonly List<SettingsUICell> m_spawnedCells = [];

    private Coroutine m_fadeInCoroutine;

    public void ShowSettings(string category)
    {
        if (m_fadeInCoroutine != null)
        {
            StopCoroutine(m_fadeInCoroutine);
            m_fadeInCoroutine = null;
        }

        foreach (SettingsUICell spawnedCell in m_spawnedCells)
            Destroy(spawnedCell.gameObject);
        
        m_spawnedCells.Clear();
        RefreshSettings();

        foreach (IExposedSetting item in from setting in settings
                                         where setting.GetCategory() == category.ToString()
                                         where setting is not IConditionalSetting conditionalSetting || conditionalSetting.ShouldShow()
                                         select setting)
        {
            SettingsUICell component = Instantiate(PeakTemplates.SettingsCellPrefab, ContentParent).GetComponent<SettingsUICell>();
            m_spawnedCells.Add(component);
            component.Setup(item as Setting);
        }

        m_fadeInCoroutine = StartCoroutine(FadeInCells());
    }

    public void RefreshSettings()
    {
        if (GameHandler.Instance != null)
            settings = GameHandler.Instance.SettingsHandler.GetSettingsThatImplements<IExposedSetting>();
        
    }

    private IEnumerator FadeInCells()
    {
        int i = 0;
        foreach (SettingsUICell spawnedCell in m_spawnedCells)
        {
            spawnedCell.FadeIn();
            yield return new WaitForSecondsRealtime(0.05f);
            i++;
        }

        m_fadeInCoroutine = null;
    }
}

public class ModdedSettingsTABS : TABS<ModdedTABSButton>
{
    public ModdedSettingsMenu SettingsMenu;

    public override void OnSelected(ModdedTABSButton button)
    {
        this.SettingsMenu.ShowSettings(button.category);
    }
}

public class ModdedTABSButton : TAB_Button
{
    public string category;
    public GameObject SelectedGraphic;

    private void Update()
    {
        text.color = Color.Lerp(text.color, this.Selected ? Color.black : Color.white, Time.unscaledDeltaTime * 7f);
        if (SelectedGraphic != null)
            SelectedGraphic.gameObject.SetActive(this.Selected);
    }
}
