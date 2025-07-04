using System;
using TMPro;
using UnityEngine;
using Zorro.Core;
using Zorro.Settings;
using Zorro.Settings.UI;
using System.Collections;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using System.Linq;

namespace EZConfig.SettingOptions;

public class BepInExKeyCode(string displayName, string category, KeyCode defaultValue = KeyCode.None, KeyCode currentValue = KeyCode.None,
    Action<KeyCode>? saveCallback = null,
    Action<BepInExKeyCode>? onApply = null) : Setting, IExposedSetting
{
    public KeyCode Value { get; set; }

    private static GameObject? _settingUICell = null;
    public static GameObject? SettingUICell
    {
        get
        {
            if (_settingUICell == null)
            {
                if (SingletonAsset<InputCellMapper>.Instance == null || SingletonAsset<InputCellMapper>.Instance.FloatSettingCell == null)
                    return null;    

                _settingUICell = Object.Instantiate(SingletonAsset<InputCellMapper>.Instance.FloatSettingCell);
                _settingUICell.name = "BepInExKeyCodeCell";

                var oldFloatSetting = _settingUICell.GetComponent<FloatSettingUI>();
                var newKeyCodeSetting = _settingUICell.AddComponent<KeyCodeSettingUI>();

                var inputRectTransform = oldFloatSetting.inputField.GetComponent<RectTransform>();
                inputRectTransform.pivot = new Vector2(0.5f, 0.5f);
                inputRectTransform.offsetMin = new Vector2(20, -25);
                inputRectTransform.offsetMax = new Vector2(380, 25);

                newKeyCodeSetting.button = _settingUICell.AddComponent<Button>();

                oldFloatSetting.inputField.name = "Button";

                Object.DestroyImmediate(oldFloatSetting.inputField.placeholder.gameObject);
                Object.Destroy(oldFloatSetting.inputField);
                Object.DestroyImmediate(oldFloatSetting.slider.gameObject);
                Object.DestroyImmediate(oldFloatSetting);


                var text = newKeyCodeSetting.button.GetComponentInChildren<TextMeshProUGUI>();
                text.fontSize = text.fontSizeMin = text.fontSizeMax = 22;
                text.alignment = TextAlignmentOptions.Center;
                newKeyCodeSetting.text = text;

                Object.DontDestroyOnLoad(_settingUICell);
            }

            return _settingUICell;
        }
    }

    public override void Load(ISettingsSaveLoad loader)
    {
        Value = currentValue;
    }

    public override void Save(ISettingsSaveLoad saver)
    {
        saveCallback?.Invoke(Value);
    }

    public override void ApplyValue()
    {
        onApply?.Invoke(this);
    }

    public override GameObject? GetSettingUICell() => SettingUICell;

    public string GetCategory() => category;

    public string GetDisplayName() => displayName;

    protected KeyCode GetDefaultValue() => defaultValue;

    public override Zorro.Settings.DebugUI.SettingUI GetDebugUI(ISettingHandler settingHandler)
    {
        return null;
    }

    public void SetValue(KeyCode newValue, ISettingHandler settingHandler)
    {
        Value = newValue;
        ApplyValue();
        settingHandler.SaveSetting(this);
    }
}

public class KeyCodeSettingUI : SettingInputUICell
{
    //public TMP_InputField inputField;
    public Button button;
    public TextMeshProUGUI text;

    public override void Setup(Setting setting, ISettingHandler settingHandler)
    {
        if (setting == null || setting is not BepInExKeyCode keyCodeSetting) return;

        RegisterSettingListener(setting);
        text.text = keyCodeSetting.Value.ToString();

        button.onClick.AddListener(() =>
        {
            StartKeybindCapture(keyCodeSetting, settingHandler);
        });
    }

    protected override void OnSettingChangedExternal(Setting setting)
    {
        base.OnSettingChangedExternal(setting);

        if (setting is BepInExKeyCode keyCode)
            text.text = keyCode.Value.ToString();

    }

    protected override void OnDestroy()
    {
        if (detectingKey == this)
            detectingKey = null;
    }

    private static KeyCodeSettingUI? detectingKey = null;

    public IEnumerator WaitForKey(System.Action<KeyCode> onKeyDetected)
    {
        // Wait for key down
        while (detectingKey != null)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    onKeyDetected?.Invoke(key);
                    yield break;
                }
            }
            yield return null;
        }
    }

    internal static KeyCode[] BlackListed = [KeyCode.Escape];
    public void StartKeybindCapture(BepInExKeyCode setting, ISettingHandler settingHandler)
    {
        if (detectingKey != null) return;

        detectingKey = this;
        text.text = "SELECT A KEY";

        StartCoroutine(WaitForKey(key => {
          
            Debug.Log($"Key pressed: {key}");

            if (!BlackListed.Contains(key))
                setting.SetValue(key, settingHandler);

            OnSettingChangedExternal(setting);
            detectingKey = null;
        }));
    }
}
