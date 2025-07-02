using Zorro.UI;

namespace EZConfig.UI
{
    public class ModSettingsPage : UIPage, IHaveParentPage
    {
        internal static string SelectedMod = "";
        public (UIPage, PageTransistion) GetParentPage()
        {
            return (pageHandler.GetPage<MainMenuSettingsPage>(), new SetActivePageTransistion());
        }

        public override void OnPageExit()
        {
            pageHandler.TransistionToPage<MainMenuSettingsPage>();
            base.OnPageExit();
        }
    }
}
