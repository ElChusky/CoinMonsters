using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "KeyBindings", menuName = "KeyBindings/Create new KeyBindings")]
public class KeyBindings : ScriptableObject
{

    public List<KeyBindingCheck> keybindingChecks;

}

[System.Serializable]
public class KeyBindingCheck
{
    public KeybindingActions keybindingAction;
    public KeyCode keyCode;
}
