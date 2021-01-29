using System;
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

    //Base stats
    private int maxHp;
    private int attack;
    private int defense;
    private int spAttack;
    private int spDefense;
    private int speed;

    private List<Tuple<int, Move>> learnableMoves = new List<Tuple<int, Move>>();

    public BasePokemon(string name, int id, Sprite frontSprite, Sprite backSprite, PokemonType type1, PokemonType type2, int maxHp, int attack, int defense, int spAttack, int spDefense, int speed, List<Tuple<int, Move>> learnableMoves)
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
        this.learnableMoves = learnableMoves;
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
    public List<Tuple<int, Move>> LearnableMoves { get => learnableMoves; set => learnableMoves = value; }
    public int Id { get => id; set => id = value; }
    public PokemonType Type1 { get => type1; set => type1 = value; }
    public PokemonType Type2 { get => type2; set => type2 = value; }
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
    Dark,
    Steel
}

public class TypeChart
{
    static float[][] typeChart =
    {   //                     NORMAL  FIRE  WATER  ELEC  GRASS  ICE  FIGHT  POIS  GROUND  FLY  PSY  BUG  ROC   GHO  DRA  DARK  STEEL

        /*NORMAL*/ new float[]{    1f,   1f,    1f,   1f,    1f,  1f,    1f,   1f,     1f,  1f,  1f,  1f,  .5f,  0f,  1f,  1f,  .5f}, 

        /*FIRE*/   new float[]{    1f,  .5f,   .5f,   1f,    2f,  2f,    1f,   1f,     1f,  1f,  1f,  2f,  .5f,  1f, .5f,  1f,   2f},

        /*WATER*/  new float[]{    1f,   2f,   .5f,   1f,   .5f,  1f,    1f,   1f,     2f,  1f,  1f,  1f,   2f,  1f, .5f,  1f,   1f},

        /*ELECTR*/ new float[]{    1f,   1f,    2f,  .5f,   .5f,  1f,    1f,   1f,     0f,  2f,  1f,  1f,   1f,  1f, .5f,  1f,   1f},

        /*GRASS*/  new float[]{    1f,  .5f,    2f,   1f,   .5f,  1f,    1f,  .5f,     2f, .5f,  1f, .5f,   2f,  1f, .5f,  1f,  .5f},

        /*ICE*/    new float[]{    1f,  .5f,   .5f,   1f,    2f, .5f,    1f,   1f,     2f,  2f,  1f,  1f,   1f,  1f,  2f,  1f,  .5f},

        /*FIGHT*/  new float[]{    2f,   1f,    1f,   1f,    1f,  2f,    1f,  .5f,     1f, .5f, .5f, .5f,   2f,  0f,  1f,  2f,   2f},

        /*POISON*/ new float[]{    1f,   1f,    1f,   1f,    2f,  1f,    1f,  .5f,    .5f,  1f,  1f,  1f,  .5f, .5f,  1f,  1f,   0f},

        /*GROUND*/ new float[]{    1f,   2f,    1f,   2f,   .5f,  1f,    1f,   2f,     1f,  0f,  1f, .5f,   1f,  1f,  1f,  1f,   2f},

        /*FLYING*/ new float[]{    1f,   1f,    1f,  .5f,    2f,  1f,    2f,   1f,     1f,  1f,  1f,  2f,  .5f,  1f,  1f,  1f,  .5f},

        /*PSY*/    new float[]{    1f,   1f,    1f,   1f,    1f,  1f,    2f,   2f,     1f,  1f, .5f,  1f,   1f,  1f,  1f,  0f,  .5f},

        /*BUG*/    new float[]{    1f,  .5f,    1f,   1f,    2f,  1f,   .5f,  .5f,     1f, .5f,  2f,  1f,   1f, .5f,  1f,  2f,  .5f},

        /*ROC*/    new float[]{    1f,   2f,    1f,   1f,    1f,  2f,   .5f,   1f,    .5f,  2f,  1f,  2f,   1f,  1f,  1f,  1f,  .5f},

        /*GHOST*/  new float[]{    0f,   1f,    1f,   1f,    1f,  1f,    1f,   1f,     1f,  1f,  2f,  1f,   1f,  2f,  1f, .5f,  .5f},

        /*DRAGON*/ new float[]{    1f,   1f,    1f,   1f,    1f,  1f,    1f,   1f,     1f,  1f,  1f,  1f,   1f,  1f,  2f,  1f,  .5f},

        /*DARK*/   new float[]{    1f,   1f,    1f,   1f,    1f,  1f,   .5f,   1f,     1f,  1f,  2f,  1f,   1f,  2f,  1f, .5f,  .5f},

        /*STEEL*/  new float[]{    1f,  .5f,   .5f,  .5f,    1f,  2f,    1f,   1f,     1f,  1f,  1f,  1f,   2f,  1f,  1f,  1f,  .5f}
    };

    public static float GetEffectiveness(PokemonType attackType, Pokemon defender)
    {
        int attackTypeRow = (int)attackType - 1;

        int defenserType1Col = (int)defender.BasePoke.Type1 - 1;
        int defenserType2Col = (int)defender.BasePoke.Type2 - 1;

        float type1Multiplier = typeChart[attackTypeRow][defenserType1Col];
        float type2Multiplier;

        if(defenserType2Col == -1)
        {
            type2Multiplier = 1f;
        } else
        {
            type2Multiplier = typeChart[attackTypeRow][defenserType2Col];
        }

        float finalMultiplier = type1Multiplier * type2Multiplier;

        return finalMultiplier;
    }
}
