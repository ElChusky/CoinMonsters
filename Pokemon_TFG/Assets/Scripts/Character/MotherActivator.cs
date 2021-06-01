using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherActivator : MonoBehaviour, IPlayerTriggerable
{

    [SerializeField] MotherNPC mother;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.IsRunning = false;
        player.Character.IsMoving = false;
        player.Character.HandleUpdate();
        GameController.Instance.OnEnterMotherActivator(mother);
    }
}
