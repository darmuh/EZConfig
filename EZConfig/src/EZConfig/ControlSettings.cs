using HarmonyLib;
using UnityEngine.InputSystem;

namespace EZConfig
{
    internal class ControlSettings
    {
        static int timesRan = 0;
        static bool settingsAdded = false;
        internal static void InitControls()
        {
            if(settingsAdded) 
                return;
            Plugin.Spam("initializing controls!");
            timesRan = 0;
            //CreateSettingForEachBinding(InputSystem.actions.FindAction("Move"));
            
            CreateSettingForEachBinding(InputSystem.actions.FindAction("Crouch"));
            CreateSettingForEachBinding(InputSystem.actions.FindAction("Drop"));
            CreateSettingForEachBinding(InputSystem.actions.FindAction("Emote"));
            CreateSettingForEachBinding(InputSystem.actions.FindAction("Interact"));
            CreateSettingForEachBinding(InputSystem.actions.FindAction("Jump"));
            CreateSettingForEachBinding(InputSystem.actions.FindAction("Pause"));
            CreateSettingForEachBinding(InputSystem.actions.FindAction("Ping"));
            //CreateSettingForEachBinding(InputSystem.actions.FindAction("Scroll"));
            //CreateSettingForEachBinding(InputSystem.actions.FindAction("SelectBackpack"));
            //CreateSettingForEachBinding(InputSystem.actions.FindAction("SelectSlotBackward"));
            //CreateSettingForEachBinding(InputSystem.actions.FindAction("SelectSlotForward"));
            CreateSettingForEachBinding(InputSystem.actions.FindAction("SpectateLeft"));
            CreateSettingForEachBinding(InputSystem.actions.FindAction("SpectateRight"));
            CreateSettingForEachBinding(InputSystem.actions.FindAction("Sprint"));
            //CreateSettingForEachBinding(InputSystem.actions.FindAction("UsePrimary"));
            //CreateSettingForEachBinding(InputSystem.actions.FindAction("UseSecondary"));
            settingsAdded = true;
        }

        internal static void CreateSettingForEachBinding(InputAction inputAction)
        {
            timesRan++;
            if (inputAction == null)
            {
                Plugin.ERROR($"INPUT ACTION WAS NULL at {timesRan}");
                return;
            }

            Plugin.Spam($"Creating setting for control - {inputAction.name}");
            inputAction.bindings.Do(b => SettingsHandler.Instance.AddSetting(new KeybindSetting(inputAction, inputAction.bindings.IndexOf(d => d == b))));
        }

        internal static void CreateSettingForEachBinding(InputAction[] inputActions)
        {
            foreach(InputAction inputAction in inputActions)
            {
                if (inputAction == null)
                    continue;
                Plugin.Spam($"Creating setting for control - {inputAction.name}");
                inputAction.bindings.Do(b => SettingsHandler.Instance.settings.Add(new KeybindSetting(inputAction, inputAction.bindings.IndexOf(d => d == b))));
            }
        }
    }
}
