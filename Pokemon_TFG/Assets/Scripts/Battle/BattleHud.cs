using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    public Text monsterName;
    public Text monsterLevel;
    public Text currentHP;
    public Text maxHP;
    public Image statusImage;
    public Sprite burnSprite;
    public Sprite poisonSprite;
    public Sprite paralSprite;
    public Sprite freezeSprite;
    public Sprite sleepSprite;
    public HP_Bar_Control hpBar;
    public GameObject expBar;
    public bool isPlayer;

    private Monster monster;

    public void SetData(Monster mon)
    {
        monster = mon;
        monsterName.text = monster.BaseMonster.Name;
        monsterLevel.text = monster.Level.ToString();
        hpBar.SetHp((float )monster.CurrentHP / monster.MaxHp);
        SetExp();

        if (isPlayer)
        {
            currentHP.text = monster.CurrentHP.ToString();
            maxHP.text = monster.MaxHp.ToString();
        }

        SetStatusImage();

        monster.OnStatusChanged += SetStatusImage;
    }

    public void SetStatusImage()
    {
        if(monster.Status == null)
        {
            statusImage.gameObject.SetActive(false);
        }
        else{
            ConditionID id = monster.Status.ID;
            switch (id)
            {
                case ConditionID.brn:
                    statusImage.sprite = burnSprite;
                    break;
                case ConditionID.psn:
                    statusImage.sprite = poisonSprite;
                    break;
                case ConditionID.par:
                    statusImage.sprite = paralSprite;
                    break;
                case ConditionID.frz:
                    statusImage.sprite = freezeSprite;
                    break;
                case ConditionID.slp:
                    statusImage.sprite = sleepSprite;
                    break;
            }
            statusImage.gameObject.SetActive(true);
        }
    }

    public IEnumerator UpdateHP()
    {
        if (isPlayer)
        {
            currentHP.text = monster.CurrentHP.ToString();
            maxHP.text = monster.MaxHp.ToString();
        }

        if (monster.HpChanged)
        {
            yield return hpBar.SetHPSmoothly((float)monster.CurrentHP / monster.MaxHp);
            monster.HpChanged = false;
        }
    }

    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmoothly()
    {
        if (expBar == null) yield break;

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    private float GetNormalizedExp()
    {
        int currentLevelExp = monster.BaseMonster.GetExpForLevel(monster.Level);
        int nextLevelExp = monster.BaseMonster.GetExpForLevel(monster.Level + 1);

        float normalizedExp = (float)(monster.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);

        return Mathf.Clamp01(normalizedExp);
    }
}
