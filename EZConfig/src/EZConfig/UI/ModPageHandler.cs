using Zorro.UI;

namespace EZConfig.UI
{
    public class ModPageHandler : UIPageHandler
    {
        internal static ModPageHandler Instance = null!;

        internal void Awake()
        {
            Instance = this;
        }
        internal void OpenModSettings(string name)
        {
            ModSettingsPage.SelectedMod = name;
            TransistionToPage<ModSettingsPage>(new SetActivePageTransistion());
        }
    }
}
