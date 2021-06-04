using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//Tp player to a diff position without switching scenes
public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationID destinationPortal;

    private PlayerController player;

    private Fader fader;

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        player.Character.Animator.IsRunning = false;
        this.player = player;
        StartCoroutine(Teleport());
    }

    IEnumerator Teleport() { 

        GameController.Instance.PauseGame(true);

        yield return fader.FadeIn(0.5f);

        LocationPortal destinationPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);

        player.Character.SetPositionAndSnapToTile(destinationPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);

        GameController.Instance.PauseGame(false);
    }

    public Transform SpawnPoint => spawnPoint;
}
