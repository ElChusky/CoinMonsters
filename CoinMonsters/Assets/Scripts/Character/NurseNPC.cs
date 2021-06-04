using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NurseNPC : MonoBehaviour, Interactable
{

    public Dialog dialogAskForHeal;
    public Dialog dialogHealConfirmed;
    public Dialog dialogHealCanceled;

    public HealingParty healing;

    private void Awake()
    {
        healing = GetComponent<HealingParty>();

        GameController.Instance.Nurse = this;

    }

    public void Interact(Transform initiator)
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialogAskForHeal, () =>
        {
            GameController.Instance.confirmHealMenu.OpenMenu();
            GameController.Instance.State = GameState.ConfirmHeal;
        }));
    }

}
