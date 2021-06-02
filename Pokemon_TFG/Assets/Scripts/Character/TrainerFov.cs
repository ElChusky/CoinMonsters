using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFov : MonoBehaviour, IPlayerTriggerable
{

    [SerializeField] AudioClip newMusic;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        GameController.Instance.prevMusic = audioManager.audioSource.clip;
        audioManager.ChangeMusic(newMusic);
        player.Character.IsRunning = false;
        player.Character.IsMoving = false;
        player.Character.HandleUpdate();
        GameController.Instance.OnEnterTrainersView(GetComponentInParent<TrainerController>());
    }

    public AudioClip TrainerMusic { get { return newMusic; } }
    public AudioManager AudioManager { get { return audioManager; } }
}
