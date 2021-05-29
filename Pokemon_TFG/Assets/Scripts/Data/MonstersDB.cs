using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstersDB
{
    static Dictionary<string, BaseMonster> monsters;

    public static void Init()
    {
        monsters = new Dictionary<string, BaseMonster>();
        BaseMonster[] monstersArray = Resources.LoadAll<BaseMonster>("");

        foreach (BaseMonster monster in monstersArray)
        {
            monsters[monster.Name] = monster;
        }
    }

    public static BaseMonster GetMonsterByName(string name)
    {
        return monsters[name];
    }
}
