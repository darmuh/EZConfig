using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Zorro.Settings;
using Zorro.Settings.UI;
using static UnityEngine.Rendering.DebugUI;

namespace EZConfig
{
    public class KeybindSetting(InputAction action, int bindIndex) : StringSetting, IExposedSetting
    {
        public InputAction inputAction = action;
        public int bindingIndex = bindIndex;

        public override GameObject GetSettingUICell()
        {
            if(Extensions.StringPrefab == null)
            {
                Extensions.StringPrefab = Object.Instantiate(InputCellMapper.Instance.FloatSettingCell);
                Extensions.StringPrefab.name = "ModdedStringUI";
                FloatSettingUI fsetting = Extensions.StringPrefab.GetComponentInChildren<FloatSettingUI>();
                Object.Destroy(fsetting.slider.gameObject);
                Object.Destroy(fsetting);
                TMP_InputField inputField = Extensions.StringPrefab.GetComponentInChildren<TMP_InputField>();
                inputField.contentType = TMP_InputField.ContentType.Standard;
                RectTransform rect = inputField.gameObject.GetComponent<RectTransform>();
                rect.sizeDelta = new(350, 50);
                rect.localPosition = Vector3.zero;
                StringSettingUI stringUI = inputField.gameObject.AddComponent<StringSettingUI>();
                stringUI.inputField = inputField;
                //TextMeshProUGUI text = inputField.gameObject.GetComponentInChildren<TextMeshProUGUI>();
                //text.gameObject.AddComponent<StringSettingUI>();
            }
            
            return Extensions.StringPrefab;
        }

        public void Submit(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            if (InputSystem.FindControl(value) == null)
                return;

            Value = value;
            inputAction.ChangeBinding(bindingIndex).WithPath(Value);
        }

        public override void ApplyValue()
        {
            Plugin.Spam("Apply value!");
            if (string.IsNullOrEmpty(Value))
                return;

            if (InputSystem.FindControl(Value) == null)
                return;

            inputAction.ChangeBinding(bindingIndex).WithPath(Value);
        }

        public void SetToDefault()
        {
            SetValue(GetDefaultValue(), SettingsHandler.Instance);
        }

        public string GetCategory()
        {
            return "Controls";
        }

        public string GetDisplayName()
        {
            return $"{inputAction.bindings[bindingIndex].groups}:{inputAction.bindings[bindingIndex].action}";
        }

        protected override string GetDefaultValue()
        {
            if (string.IsNullOrEmpty(inputAction.bindings[bindingIndex].path))
                return "NOT SET";

            return inputAction.bindings[bindingIndex].path;
        }
    }
}
