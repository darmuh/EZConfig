using EZConfig.Components;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;
using Zorro.Settings;
using Zorro.Settings.UI;

namespace EZConfig.UI;
#nullable disable
internal static class PeakTemplates
{
    internal static GameObject ButtonTemplate { get; private set; }
    //internal static GameObject PauseButtonTemplate { get; private set; }
    public static GameObject SettingsCellPrefab { get; internal set; }

    private static GameObject _textInput;
    public static GameObject TextInput
    {
        get
        {
            if (_textInput == null)
            {
                if (SingletonAsset<InputCellMapper>.Instance == null || SingletonAsset<InputCellMapper>.Instance.FloatSettingCell == null)
                    return null;

                _textInput = Object.Instantiate(SingletonAsset<InputCellMapper>.Instance.FloatSettingCell);
                _textInput.name = "PeakTextInput";

                var oldFloatSetting = _textInput.GetComponent<FloatSettingUI>();
                var inputField = oldFloatSetting.inputField;

                Object.DestroyImmediate(oldFloatSetting.slider.gameObject);
                Object.DestroyImmediate(oldFloatSetting);

                inputField.characterValidation = TMP_InputField.CharacterValidation.None;
                var inputRectTransform = inputField.GetComponent<RectTransform>();

                inputRectTransform.pivot = new Vector2(0.5f, 0.5f);
                UIUtilities.ExpandToParent(inputRectTransform);

                var texts = inputField.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var text in texts)
                {
                    text.fontSize = text.fontSizeMin = text.fontSizeMax = 22;
                    text.alignment = TextAlignmentOptions.MidlineLeft;
                }

                Object.DontDestroyOnLoad(_textInput);
            }

            return _textInput;
        }
    }

    [HarmonyPatch(typeof(MenuWindow), nameof(MenuWindow.Start))]
    private class PauseMainMenu_Initialize
    {
        static bool Prefix(MenuWindow __instance)
        {
            if (__instance is not MainMenu menu) return true;

            var settingsMenu = menu.settingsMenu as PauseMainMenu;
            var sharedSettings = settingsMenu.GetComponentInChildren<SharedSettingsMenu>();

            if (sharedSettings != null)
            {
                Plugin.WARNING("Found m_settingsCellPrefab");
                SettingsCellPrefab = sharedSettings.m_settingsCellPrefab;
            }


            if (ButtonTemplate == null)
            {
                ButtonTemplate = Object.Instantiate(settingsMenu.backButton.gameObject);
                ButtonTemplate.name = "PeakUIButton";

                Object.DontDestroyOnLoad(ButtonTemplate);
            }

            return true;
        }
    }


    //[HarmonyPatch(typeof(GUIManager), nameof(GUIManager.Awake))]
    //private class GUIManager_Awake
    //{
    //    static void Postfix(GUIManager __instance)
    //    {
    //        var pauseMenu = __instance.pauseMenu as PauseOptionsMenu;
  
    //        if (PauseButtonTemplate == null)
    //        {
    //            PauseButtonTemplate = Object.Instantiate(pauseMenu.resumeButton.gameObject);
    //            PauseButtonTemplate.name = "PeakUIPauseButton";

    //            Object.DontDestroyOnLoad(PauseButtonTemplate);
    //        }
    //    }
    //}
}
