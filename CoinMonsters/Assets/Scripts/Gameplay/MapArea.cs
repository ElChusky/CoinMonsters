using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    //Dictionary<rarityLvl, Monster>
    [SerializeField] List<MonsterWithRarity> wildMonsters;

    public int Rarity { get; set; }

    public Monster GetWildMonster()
    {

        List<Monster> monstersWithRarity = new List<Monster>();
        //We add all the Monsters with the given rarity in the area into a List
        do
        {
            foreach (MonsterWithRarity pair in wildMonsters)
            {
                if (pair.Rarity == Rarity)
                {
                    monstersWithRarity.Add(pair._Monster);
                }
            }
            Rarity--;
        } while (monstersWithRarity.Count == 0 && Rarity != 0);

        if (monstersWithRarity.Count == 0)
            return null;

        //We return a random Monster of the list
        Monster monster = monstersWithRarity[Random.Range(0, monstersWithRarity.Count)];
        monster.Init();
        return monster;
    }
}
