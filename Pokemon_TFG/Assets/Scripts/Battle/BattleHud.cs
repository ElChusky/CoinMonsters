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
    public HP_Bar_Control hpBar;
    public bool isPlayer;

    private Monster monster;

    public void SetData(Monster mon)
    {
        monster = mon;
        monsterName.text = monster.BaseMonster.Name;
        monsterLevel.text = monster.Level.ToString();
        hpBar.SetHp((float )monster.CurrentHP / monster.MaxHp);

        if (isPlayer)
        {
            currentHP.text = monster.CurrentHP.ToString();
            maxHP.text = monster.MaxHp.ToString();
        }
    }

    public IEnumerator UpdateHP()
    {
        if (isPlayer)
        {
            currentHP.text = monster.CurrentHP.ToString();
            maxHP.text = monster.MaxHp.ToString();
        }

        yield return hpBar.SetHPSmoothly((float)monster.CurrentHP / monster.MaxHp);

    }
}
