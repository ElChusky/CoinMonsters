﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster/Create new Monster")]
public class BaseMonster : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] int id;

    [SerializeField] Sprite sprite;

    [SerializeField] MonsterType type1;
    [SerializeField] MonsterType type2;

    //Base stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves = new List<LearnableMove>();

    public BaseMonster(string name, int id, Sprite sprite, MonsterType type1, MonsterType type2, int maxHp, int attack, int defense, int spAttack, int spDefense, int speed, List<LearnableMove> learnableMoves)
    {
        this.name = name;
        this.id = id;
        this.sprite = sprite;
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

    public enum Stat
    {
        Attack,
        Defense,
        SpAttack,
        SpDefense,
        Speed,

        //Not a boostable stat, but attacks that restore HP will be here
        HP,

        //Not actual stats, but buffs or debuffs for moveAccuracy
        Accuracy,
        Evasion
    }

    public string Name { get => name; set => name = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int SpAttack { get => spAttack; set => spAttack = value; }
    public int SpDefense { get => spDefense; set => spDefense = value; }
    public int Speed { get => speed; set => speed = value; }
    public Sprite Sprite { get => sprite; set => sprite = value; }
    public List<LearnableMove> LearnableMoves { get => learnableMoves; set => learnableMoves = value; }
    public int Id { get => id; set => id = value; }
    public MonsterType Type1 { get => type1; set => type1 = value; }
    public MonsterType Type2 { get => type2; set => type2 = value; }
}

public enum MonsterType
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

    public static float GetEffectiveness(MonsterType attackType, Monster defender)
    {
        int attackTypeRow = (int)attackType - 1;

        int defenserType1Col = (int)defender.BaseMonster.Type1 - 1;
        int defenserType2Col = (int)defender.BaseMonster.Type2 - 1;

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