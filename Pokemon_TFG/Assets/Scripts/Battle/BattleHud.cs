using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    public Text pokemonName;
    public Text pokemonLevel;
    public Text currentHP;
    public Text maxHP;
    public HP_Bar_Control hpBar;
    public bool isPlayer;

    private Pokemon pokemon;

    public void SetData(Pokemon poke)
    {
        pokemon = poke;
        pokemonName.text = pokemon.BasePoke.Name;
        pokemonLevel.text = pokemon.Level.ToString();
        hpBar.SetHp((float )pokemon.CurrentHP / pokemon.MaxHp);

        if (isPlayer)
        {
            currentHP.text = pokemon.CurrentHP.ToString();
            maxHP.text = pokemon.MaxHp.ToString();
        }
    }

    public IEnumerator UpdateHP()
    {
        if (isPlayer)
        {
            currentHP.text = pokemon.CurrentHP.ToString();
            maxHP.text = pokemon.MaxHp.ToString();
        }

        yield return hpBar.SetHPSmoothly((float)pokemon.CurrentHP / pokemon.MaxHp);

    }
}
