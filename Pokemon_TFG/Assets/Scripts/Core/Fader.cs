using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    private Image fader;

    private void Awake()
    {
        fader = GetComponent<Image>();
    }

    public IEnumerator FadeIn(float time)
    {
        yield return fader.DOFade(1f, time).WaitForCompletion();
    }

    public IEnumerator FadeOut(float time)
    {
        yield return fader.DOFade(0f, time).WaitForCompletion();
    }

}
