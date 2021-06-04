using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;

    public void ChangeMusic(AudioClip newMusic)
    {
        if (audioSource.clip != null && audioSource.clip.name == newMusic.name)
            return;
        audioSource.Stop();
        audioSource.clip = newMusic;
        audioSource.Play();
    }

}
