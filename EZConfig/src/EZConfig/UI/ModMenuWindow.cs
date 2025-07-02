using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Settings;
using static EZConfig.Patching;
using Object = UnityEngine.Object;

namespace EZConfig.UI
{
    public class ModMenuWindow : MonoBehaviour
    {
        public ModSettingTabsMenu modTabs = null!;

        public GameObject m_settingsCellPrefab = null!;

        public Transform m_settingsContentParent = null!;

        public List<IExposedSetting> modSettings = [];

        public readonly List<SettingsUICell> m_spawnedCells = [];

        public Coroutine fadeInCoroutine = null!;

        public void OnEnable()
        {
            if(modTabs == null || m_settingsCellPrefab == null)
            {
                Plugin.WARNING("Unable to open modmenuwindow! NULL REFS!");
                return;
            }

            RefreshSettings();
            if (modTabs.selectedButton != null)
            {
                modTabs.Select(modTabs.selectedButton);
            }
        }

        public void RefreshSettings()
        {
            if (GameHandler.Instance != null)
            {
                modSettings = GameHandler.Instance.SettingsHandler.GetSettingsThatImplements<IExposedSetting>();
            }

            modTabs.UpdateTabs(ModSettingsPage.SelectedMod);
        }

        public void ShowSettings(string sectionName)
        {
            if (fadeInCoroutine != null)
            {
                StopCoroutine(fadeInCoroutine);
                fadeInCoroutine = null!;
            }

            foreach (SettingsUICell spawnedCell in m_spawnedCells)
            {
                Object.Destroy(spawnedCell.gameObject);
            }

            m_spawnedCells.Clear();
            RefreshSettings();
            foreach (IExposedSetting item in from setting in modSettings
                                             where setting.GetCategory() == sectionName
                                             where setting is not IConditionalSetting conditionalSetting || conditionalSetting.ShouldShow()
                                             select setting)
            {
                SettingsUICell component = Object.Instantiate(m_settingsCellPrefab, m_settingsContentParent).GetComponent<SettingsUICell>();
                m_spawnedCells.Add(component);
                component.Setup(item as Setting);
            }

            fadeInCoroutine = StartCoroutine(FadeInCells());
        }

        public IEnumerator FadeInCells()
        {
            int i = 0;
            foreach (SettingsUICell spawnedCell in m_spawnedCells)
            {
                spawnedCell.FadeIn();
                yield return new WaitForSecondsRealtime(0.05f);
                i++;
            }

            fadeInCoroutine = null;
        }
    }
}
