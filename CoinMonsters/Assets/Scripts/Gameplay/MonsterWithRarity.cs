using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterWithRarity
{

    [SerializeField] Monster monster;
    [SerializeField] int rarity;

    public Monster _Monster
    {
        get
        {
            monster.Init();
            return monster;
        }
    }
    public int Rarity { get { return rarity; } }
    
}
