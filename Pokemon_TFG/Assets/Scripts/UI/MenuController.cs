using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    [SerializeField] GameObject menu;
    [SerializeField] Color highlightedColor;
    [SerializeField] Color normalColor;

    public event Action<int> OnMenuSelected;
    public event Action OnBack;

    private List<Text> menuItems;

    private int selectedItem = 0;

    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<Text>().ToList();
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            selectedItem++;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selectedItem--;

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //Action
            OnMenuSelected?.Invoke(selectedItem);
        } else if (Input.GetKeyDown(KeyCode.X))
        {
            CloseMenu();
            OnBack?.Invoke();
        }

        UpdateItemSelection();

    }

    private void UpdateItemSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedItem)
                menuItems[i].color = highlightedColor;
            else
                menuItems[i].color = normalColor;

        }
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
    }

    public GameObject Menu { get { return menu; } }

}
