using UnityEngine.InputSystem;
using Zorro.Settings;

namespace EZConfig;

public class KeybindSetting(InputAction action, int bindIndex) : StringSetting, IExposedSetting
{
    public InputAction inputAction = action;
    public int bindingIndex = bindIndex;

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
