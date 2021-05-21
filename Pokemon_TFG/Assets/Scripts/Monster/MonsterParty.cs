using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] List<Monster> monsters;

    public List<Monster> Monsters { get; set; }

    private void Start()
    {
        foreach (Monster monster in monsters)
        {
            monster.Init();
        }
    }

    public Monster GetHealthyPokemon()
    {
        return monsters.Where(x => x.CurrentHP > 0).FirstOrDefault();
    }

}
