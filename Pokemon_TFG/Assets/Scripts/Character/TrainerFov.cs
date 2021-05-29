using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFov : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.IsRunning = false;
        player.Character.IsMoving = false;
        player.Character.HandleUpdate();
        GameController.Instance.OnEnterTrainersView(GetComponentInParent<TrainerController>());
    }
}
