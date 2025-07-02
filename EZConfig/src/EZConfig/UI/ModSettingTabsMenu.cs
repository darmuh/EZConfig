using HarmonyLib;
using UnityEngine;
using Zorro.UI;

namespace EZConfig.UI
{
    public class ModSettingTabsMenu : TABS<ModTabMenuButton>
    {
        public ModMenuWindow ModMenu = null!;

        //this method errors because it's called too early I think
        public void UpdateTabs(string modName)
        {
            if(!MenuAPI.ModMenus.ContainsKey(modName))
            {
                Plugin.ERROR($"Unable to get tabs for {modName}!");
            }

            int tabs = MenuAPI.ModMenus[modName].Count;
            int tabsEnabled = buttons.FindAll(b => b.gameObject.activeSelf).Count;
            Plugin.Spam($"Expected Tabs: {tabs}\nTabs Enabled: {tabsEnabled}");
            buttons.DoIf(b => tabsEnabled > tabs, b => 
            { 
                b.gameObject.SetActive(false);
                tabsEnabled--;
                Plugin.Spam($"Disabled a tab. Tabs Enabled: {tabsEnabled}");
            });

            //make sure there's enough tabs
            int tabObjects = buttons.Count;
            Plugin.Spam($"Tab Objects: {tabObjects}");
            if (tabObjects < tabs)
            {
                GameObject newTab = Instantiate<GameObject>(buttons[0].gameObject);
                buttons.Add(newTab.GetComponent<ModTabMenuButton>());
                tabObjects = buttons.Count;
            }

            if(MenuAPI.ModMenus[modName].Count > buttons.Count)
            {
                Plugin.ERROR($"Too many sections for amount of buttons!\nSections: {MenuAPI.ModMenus[modName].Count}\nButtons: {buttons.Count}");
                return;
            }

            for (int i = 0; i < MenuAPI.ModMenus[modName].Count - 1; i++)
            {
                buttons[i].SectionName = MenuAPI.ModMenus[modName][i];
                buttons[i].gameObject.SetActive(true);
            }
        }

        public override void OnSelected(ModTabMenuButton button)
        {
            if(ModMenu == null)
            {
                Plugin.WARNING("Unable to select this tab! ModMenu is null!!!");
                return;
            }

            ModMenu.ShowSettings(button.SectionName);
        }
    }
}
