using HarmonyLib;
using System.Xml.Linq;
using Zorro.Settings;
using Zorro.UI;

namespace EZConfig.UI
{
    public class ModSettingButton(string modName) : ButtonSetting, IExposedSetting
    {
        internal string Name = modName;
        public override string GetButtonText()
        {
            return $"{Name} Settings";
        }

        public string GetCategory()
        {
            return "Mods";
        }

        public string GetDisplayName()
        {
            return $"{Name} Settings";
        }

        public override void OnClicked(ISettingHandler settingHandler)
        {
            if(ModPageHandler.Instance == null)
                MenuAPI.CreateModPageHandler();

            //open mod's specific config menu
            MenuWindow.CloseAllWindows();
            ModPageHandler.Instance.OpenModSettings(Name);
            Patching.AddToMainMenu.ModMenuPage.SetActive(true);
        }
    }
}
