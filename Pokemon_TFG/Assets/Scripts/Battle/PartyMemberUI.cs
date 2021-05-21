using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    public Text monsterName;
    public Text monsterLevel;
    public Text currentHP;
    public Text maxHP;
    public HP_Bar_Control hpBar;

    public Color highlightedColor;

    private Monster monster;

    public void SetData(Monster mon)
    {
        monster = mon;
        monsterName.text = monster.BaseMonster.Name;
        monsterLevel.text = "Lv " + monster.Level.ToString();
        hpBar.SetHp((float)monster.CurrentHP / monster.MaxHp);

        currentHP.text = monster.CurrentHP.ToString() + "/";
        maxHP.text = monster.MaxHp.ToString();
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            gameObject.GetComponent<Image>().color = highlightedColor;
        else
        {
            if(monster.CurrentHP <= 0)
                gameObject.GetComponent<Image>().color = Color.red;
            else
                gameObject.GetComponent<Image>().color = Color.white;
        }
            
    }

}
