using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] KeyBindings keybinding;
    public static InputManager Instance { get; set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public KeyCode GetKeyForAction(KeybindingActions action)
    {
        foreach (KeyBindingCheck check in keybinding.keybindingChecks)
        {
            if(check.keybindingAction == action)
            {
                return check.keyCode;
            }
        }
        return KeyCode.None;
    }

    public bool GetKeyDown(KeybindingActions key)
    {
        //When pressed down

        foreach (KeyBindingCheck check in keybinding.keybindingChecks)
        {
            if (check.keybindingAction == key)
            {
                return Input.GetKeyDown(check.keyCode);
            }
        }

        return false;
    }

    public bool GetKey(KeybindingActions key)
    {
        //When key hold down

        foreach (KeyBindingCheck check in keybinding.keybindingChecks)
        {
            if (check.keybindingAction == key)
            {
                return Input.GetKey(check.keyCode);
            }
        }

        return false;
    }

    public float GetAxisRaw(string axis)
    {
        if(axis == "Horizontal")
        {
            if (GetKey(KeybindingActions.Right))
                return 1f;
            else if (GetKey(KeybindingActions.Left))
                return -1f;
        } else if(axis == "Vertical")
        {
            if (GetKey(KeybindingActions.Up))
                return 1f;
            else if (GetKey(KeybindingActions.Down))
                return -1f;
        }
        return 0f;
    }
}

public enum KeybindingActions
{
    None,
    Up,
    Down,
    Right,
    Left,
    Run,
    OpenMenu,
    InteractAndConfirm,
    CancelAndReturn,
}
