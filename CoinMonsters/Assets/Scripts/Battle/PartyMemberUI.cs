﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    public Sprite burnSprite;
    public Sprite poisonSprite;
    public Sprite paralSprite;
    public Sprite freezeSprite;
    public Sprite sleepSprite;
    public Image statusImage;
    public Text monsterName;
    public Text monsterLevel;
    public Text currentHP;
    public Text maxHP;
    public HP_Bar_Control hpBar;

    public Image monsterSprite;

    public Color highlightedColor;
    public Color deadColor;
    private Color originalSpriteColor;
    private Color originalColor;

    private Monster monster;

    public void SetData(Monster mon)
    {
        monster = mon;
        monsterName.text = monster.BaseMonster.Name;
        monsterLevel.text = "Lv " + monster.Level.ToString();
        hpBar.SetHp((float)monster.CurrentHP / monster.MaxHp);
        monsterSprite.sprite = monster.BaseMonster.Sprite;
        originalSpriteColor = monsterSprite.color;
        originalColor = gameObject.GetComponent<Image>().color;
        currentHP.text = monster.CurrentHP.ToString() + "/";
        maxHP.text = monster.MaxHp.ToString();
        SetStatusImage();
    }

    public void SetStatusImage()
    {
        if (monster.Status == null)
        {
            statusImage.gameObject.SetActive(false);
        }
        else
        {
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

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            gameObject.GetComponent<Image>().color = highlightedColor;
            monsterSprite.color = highlightedColor;
        }
        else
        {
            if(monster.CurrentHP <= 0)
            {
                gameObject.GetComponent<Image>().color = deadColor;
                monsterSprite.color = deadColor;
            }
            else
            {
                gameObject.GetComponent<Image>().color = originalColor;
                monsterSprite.color = originalSpriteColor;
            }
        }
            
    }

}
