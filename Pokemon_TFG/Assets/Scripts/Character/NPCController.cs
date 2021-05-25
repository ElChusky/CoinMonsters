using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{

    [SerializeField] Dialog dialog;
    [SerializeField] PlayerController playerController;

    public void Interact()
    {
        Debug.Log("Interacting with NPC");
        playerController.dialogActive = true;
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }
}
