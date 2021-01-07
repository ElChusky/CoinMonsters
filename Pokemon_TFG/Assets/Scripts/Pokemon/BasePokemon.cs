using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasePokemon
{
    private string name;
    private int id;

    private Sprite frontSprite;
    private Sprite backSprite;

    private PokemonType type1;
    private PokemonType type2;
    private Rarity rarity;

    //Base stats
    private int maxHp;
    private int attack;
    private int defense;
    private int spAttack;
    private int spDefense;
    private int speed;

    private Dictionary<int, Move> learnableMoves = new Dictionary<int, Move>();

    public BasePokemon(string name, int id, Sprite frontSprite, Sprite backSprite, PokemonType type1, PokemonType type2, int maxHp, int attack, int defense, int spAttack, int spDefense, int speed)
    {
        this.name = name;
        this.id = id;
        this.frontSprite = frontSprite;
        this.backSprite = backSprite;
        this.type1 = type1;
        this.type2 = type2;
        this.maxHp = maxHp;
        this.attack = attack;
        this.defense = defense;
        this.spAttack = spAttack;
        this.spDefense = spDefense;
        this.speed = speed;
    }

    public string Name { get => name; set => name = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int SpAttack { get => spAttack; set => spAttack = value; }
    public int SpDefense { get => spDefense; set => spDefense = value; }
    public int Speed { get => speed; set => speed = value; }
    public Sprite FrontSprite { get => frontSprite; set => frontSprite = value; }
    public Sprite BackSprite { get => backSprite; set => backSprite = value; }
    public Dictionary<int, Move> LearnableMoves { get => learnableMoves; set => learnableMoves = value; }
    public int Id { get => id; set => id = value; }
}



public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Steel,
    Dark
}

public enum Rarity
{
    VeryCommon = 1,
    Common = 2,
    SemiRare = 3,
    Rare = 4,
    VeryRare = 5
}
