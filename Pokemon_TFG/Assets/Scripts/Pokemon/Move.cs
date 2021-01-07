using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    private string name;

    private string description;

    private PokemonType type;

    private int power;

    private int accuracy;

    private int pp;

    private string damageClass;

    public Move(string name, string description, PokemonType type, int power, int accuracy, int pp, string damageClass)
    {
        this.name = name;
        this.description = description;
        this.type = type;
        this.power = power;
        this.accuracy = accuracy;
        this.pp = pp;
        this.damageClass = damageClass;
    }

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public PokemonType Type { get => type; set => type = value; }
    public int Power { get => power; set => power = value; }
    public int Accuracy { get => accuracy; set => accuracy = value; }
    public int Pp { get => pp; set => pp = value; }
    public string DamageClass { get => damageClass; set => damageClass = value; }

    public bool Equals(Move otherMove)
    {
        if (this.name.Equals(otherMove.name))
        {
            return true;
        } else
        {
            return false;
        }
    }
}
