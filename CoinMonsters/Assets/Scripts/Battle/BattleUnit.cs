using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    private BaseMonster baseMonster;

    public bool isPlayerUnit;
    public BattleHud hud;

    public Monster Monster { get; set; }
    public BaseMonster BaseMonster { get => baseMonster; set => baseMonster = value; }

    Image image;
    Vector3 originalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Monster monster)
    {
        Monster = monster;
        BaseMonster = monster.BaseMonster;

        image.sprite = Monster.BaseMonster.Sprite;

        hud.gameObject.SetActive(true);
        hud.SetData(monster);

        transform.localScale = (isPlayerUnit) ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
        image.color = originalColor;
        PlayEnterAnimation();
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        } else
        {
            image.transform.localPosition = new Vector3(500f, originalPos.y);
        }
        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        Sequence sequence = DOTween.Sequence();

        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.grey, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }

    public IEnumerator PlayCaptureAnimation(GameObject monsterBall)
    {
        Sprite originalSprite = monsterBall.GetComponent<SpriteRenderer>().sprite;
        monsterBall.GetComponent<SpriteRenderer>().sprite = monsterBall.GetComponent<Image>().sprite;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0, 0.5f));
        sequence.Join(transform.DOMove(monsterBall.transform.position, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1), 0.5f));
        yield return sequence.WaitForCompletion();
        monsterBall.GetComponent<SpriteRenderer>().sprite = originalSprite;
    }
    public IEnumerator PlayBreakOutAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, 0.5f));
        sequence.Join(transform.DOLocalMove(originalPos, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }


}
