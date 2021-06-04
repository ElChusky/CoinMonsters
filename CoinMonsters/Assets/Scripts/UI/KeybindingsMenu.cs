using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class KeybindingsMenu : MonoBehaviour
{

    [SerializeField] KeyBindings keybindings;
    private GameObject[] keybindButtons;
    public Dictionary<KeybindingActions, KeyCode> Keybinds { get; set; }

    private string bindName;

    private void Awake()
    {
        Keybinds = new Dictionary<KeybindingActions, KeyCode>();

        keybindButtons = GameObject.FindGameObjectsWithTag("keybind");

        if(PlayerPrefs.HasKey(keybindings.keybindingChecks[0].keybindingAction.ToString())) //Significa que ya he guardado las opciones, asi que las cargamos en vez de bindear las kes por defecto
        {
            RestoreFromPlayerPrefs();
        } else
        {
            BindKey(keybindings.keybindingChecks[0].keybindingAction, keybindings.keybindingChecks[0].keyCode);
            BindKey(keybindings.keybindingChecks[1].keybindingAction, keybindings.keybindingChecks[1].keyCode);
            BindKey(keybindings.keybindingChecks[2].keybindingAction, keybindings.keybindingChecks[2].keyCode);
            BindKey(keybindings.keybindingChecks[3].keybindingAction, keybindings.keybindingChecks[3].keyCode);
            BindKey(keybindings.keybindingChecks[4].keybindingAction, keybindings.keybindingChecks[4].keyCode);
            BindKey(keybindings.keybindingChecks[5].keybindingAction, keybindings.keybindingChecks[5].keyCode);
            BindKey(keybindings.keybindingChecks[6].keybindingAction, keybindings.keybindingChecks[6].keyCode);
            BindKey(keybindings.keybindingChecks[7].keybindingAction, keybindings.keybindingChecks[7].keyCode);
        }
    }

    public void BindKey(KeybindingActions action, KeyCode keybind)
    {
        Dictionary<KeybindingActions, KeyCode> currentDictionary = Keybinds;

        if (!currentDictionary.ContainsKey(action))//Si no existe el keybind, es decir, no se ha asignado todavía
        {
            currentDictionary.Add(action, keybind);
            UpdateKeyText(action.ToString(), keybind);
        }
        else
        {
            KeybindingActions mKey = currentDictionary.FirstOrDefault(x => x.Value == keybind).Key;
            currentDictionary[mKey] = KeyCode.None;

            UpdateKeyText(action.ToString(), KeyCode.None);
        }
        currentDictionary[action] = keybind;

        int position = currentDictionary.ToList().FindIndex(v => v.Key == action);
        UpdateKeyText(action.ToString(), keybind);
        keybindings.keybindingChecks[position].keyCode = keybind;

        bindName = string.Empty;

    }

    private void SaveIntoPlayerPrefs()
    {
        for (int i = 0; i < keybindings.keybindingChecks.Count - 1; i++)
        {
            PlayerPrefs.SetInt(keybindings.keybindingChecks[i].keybindingAction.ToString(), (int)keybindings.keybindingChecks[i].keyCode);
        }
    }

    private void RestoreFromPlayerPrefs()
    {
        for (int i = 0; i < keybindings.keybindingChecks.Count - 1; i++)
        {
            KeyCode keybind = (KeyCode)PlayerPrefs.GetInt(keybindings.keybindingChecks[i].keybindingAction.ToString());
            BindKey(keybindings.keybindingChecks[i].keybindingAction, keybind);
        }
    }

    public void KeyBindOnClick(string bindName)
    {
        this.bindName = bindName;
    }

    private void OnGUI()
    {
        if(bindName != string.Empty)
        {
            Event e = Event.current;

            if (e.isKey)
            {
                KeybindingActions action;
                switch (bindName)
                {
                    case "Up":
                        action = KeybindingActions.Up;
                        break;
                    case "Down":
                        action = KeybindingActions.Down;
                        break;
                    case "Right":
                        action = KeybindingActions.Right;
                        break;
                    case "Left":
                        action = KeybindingActions.Left;
                        break;
                    case "Run":
                        action = KeybindingActions.Run;
                        break;
                    case "OpenMenu":
                        action = KeybindingActions.OpenMenu;
                        break;
                    case "InteractAndConfirm":
                        action = KeybindingActions.InteractAndConfirm;
                        break;
                    case "CancelAndReturn":
                        action = KeybindingActions.CancelAndReturn;
                        break;
                    default:
                        action = KeybindingActions.None;
                        break;
                }

                BindKey(action, e.keyCode);
                SaveIntoPlayerPrefs();
            }
        }
    }

    public void UpdateKeyText(string key, KeyCode keybind)
    {
        TMP_Text tmp = Array.Find(keybindButtons, x => x.name == key).GetComponentInChildren<TMP_Text>();
        tmp.text = keybind.ToString();
    }

}
