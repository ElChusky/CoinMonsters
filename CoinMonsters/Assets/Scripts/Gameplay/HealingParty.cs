using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingParty : MonoBehaviour
{

    public void HealParty(PlayerController player)
    {

        List<Monster> playerMonsters = player.GetComponent<MonsterParty>().Monsters;

        foreach (Monster monster in playerMonsters)
        {
            monster.HealMonster();
        }

        GameController.Instance.LastHealPosition = player.transform.position;

    }

    public static void HealAfterDeath(PlayerController player)
    {

        List<Monster> playerMonsters = player.GetComponent<MonsterParty>().Monsters;

        foreach (Monster monster in playerMonsters)
        {
            monster.HealMonster();
        }

    }
}
