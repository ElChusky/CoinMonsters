using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    private BasePokemon basePoke;
    private int level;
    private int currentHP;
    private int hpIV, attackIV, defenseIV, spAttackIV, spDefenseIV, speedIV;
    private Move[] learntMoves = new Move[4];

    public Pokemon(BasePokemon basePoke, int level)
    {
        this.BasePoke = basePoke;
        this.Level = level;
        SetIVs();
        currentHP = MaxHp;
        GiveLearntMoves();
    }

    #region Stats Formulas
    public int Attack
    {
        get { return Mathf.FloorToInt(((2 * BasePoke.Attack + AttackIV) * Level * 0.01f) + 5); }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt(((2 * BasePoke.Defense + DefenseIV) * Level * 0.01f) + 5); }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt(((2 * BasePoke.SpAttack + SpAttackIV) * Level * 0.01f) + 5); }
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt(((2 * BasePoke.SpDefense + SpDefenseIV) * Level * 0.01f) + 5); }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt(((2 * BasePoke.Speed + SpeedIV) * Level * 0.01f) + 5); }
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt(((2 * BasePoke.MaxHp + HpIV) * Level * 0.01f) + Level + 10); }
    }
    #endregion

    public BasePokemon BasePoke { get => basePoke; set => basePoke = value; }
    public int Level { get => level; set => level = value; }

    public int CurrentHP { get => currentHP; set => currentHP = value; }

    public Move[] LearntMoves { get => learntMoves; set => learntMoves = value; }

    #region Pokemon IV
    public int HpIV { get => hpIV; set => hpIV = value; }
    public int AttackIV { get => attackIV; set => attackIV = value; }
    public int DefenseIV { get => defenseIV; set => defenseIV = value; }
    public int SpAttackIV { get => spAttackIV; set => spAttackIV = value; }
    public int SpDefenseIV { get => spDefenseIV; set => spDefenseIV = value; }
    public int SpeedIV { get => speedIV; set => speedIV = value; }
    #endregion

    private void SetIVs()
    {
        HpIV = UnityEngine.Random.Range(1, 32);
        AttackIV = UnityEngine.Random.Range(1, 32);
        DefenseIV = UnityEngine.Random.Range(1, 32);
        SpAttackIV = UnityEngine.Random.Range(1, 32);
        SpDefenseIV = UnityEngine.Random.Range(1, 32);
        SpeedIV = UnityEngine.Random.Range(1, 32);
    }

    private void GiveLearntMoves()
    {
        List<Tuple<int, Move>> moves = BasePoke.LearnableMoves;
        
        int numMoves = 0;
        List<Move> learnableMoves = new List<Move>();
        foreach (Tuple<int, Move> move in moves)
        {
            if (move.Item1 <= level)
            {
                learnableMoves.Add(move.Item2);
                numMoves++;
            }
        }
        if (numMoves <= 4)
        {
            for (int i = 0; i < learnableMoves.Count; i++)
            {
                learntMoves[i] = learnableMoves[i];
            }
        }
        else
        {
            #region Codigo para dar 1 move de cada tipo, y cuando ya se hayan dado, dar en funcion de la stat mas alta hasta 4 moves si se puede. (Echar un ojo y modificar)

            bool tieneFisico = false, tieneEspecial = false, tieneStatus = false;
            //With this loop we give 1 attack of each type, physical dmg, special dmg and status move if there are any
            foreach (Move currentMove in learnableMoves)
            {
                if (currentMove.DamageClass.Equals("physical") && !tieneFisico)
                {
                    for (int i = 0; i < learntMoves.Length; i++)
                    {
                        if (learntMoves[i] == null)
                        {
                            learntMoves[i] = currentMove;
                            tieneFisico = true;
                            break;
                        }
                    }
                }
                else if (currentMove.DamageClass.Equals("special") && !tieneEspecial)
                {
                    for (int i = 0; i < learntMoves.Length; i++)
                    {
                        if (learntMoves[i] == null)
                        {
                            learntMoves[i] = currentMove;
                            tieneEspecial = true;
                            break;
                        }
                    }
                }
                else if (currentMove.DamageClass.Equals("status") && !tieneStatus)
                {
                    for (int i = 0; i < learntMoves.Length; i++)
                    {
                        if (learntMoves[i] == null)
                        {
                            learntMoves[i] = currentMove;
                            tieneStatus = true;
                        }
                    }
                }
                if (tieneFisico && tieneEspecial && tieneStatus)
                {
                    break;
                }
            }
            //Now we will check which base stat of the pokemon is higher, so we choose to give another physical dmg move (attack higher),
            //another special dmg move (sp attack higher) or another status move (hp, defense or sp defense higher)
            for (int higherStat = 0; higherStat < learntMoves.Length; higherStat++)
            {
                string damageClass = DamageClassSearch(higherStat);
                for (int index = 0; index < learnableMoves.Count; index++)
                {
                    Move currentMove = learnableMoves[index];
                    if (currentMove.DamageClass.Equals(damageClass))
                    {
                        bool learnt = false;
                        for (int i = 0; i < learntMoves.Length; i++)
                        {
                            if (learntMoves[i] == null && !learnt)
                            {
                                learntMoves[i] = currentMove;
                                index = moves.Count;
                                if (LearntMoves[4] != null)
                                {
                                    higherStat = learntMoves.Length;
                                }
                            }
                            else
                            {
                                if (currentMove.Equals(learntMoves[i]))
                                {
                                    learnt = true;
                                }
                            }
                        }
                    }
                }

            }
            #endregion
        }
    }

    private string DamageClassSearch(int highStatPos)
    {
        string damageClass = "";
        string higherStat = "";
        int[] stats = new int[5];
        stats[0] = basePoke.MaxHp;
        stats[1] = basePoke.Attack;
        stats[2] = basePoke.Defense;
        stats[3] = basePoke.SpAttack;
        stats[4] = basePoke.SpDefense;
        Array.Sort(stats);
        Array.Reverse(stats);
        int higherStatValue = stats[highStatPos];
        if (higherStatValue == basePoke.MaxHp)
        {
            higherStat = "hp";
        }
        else if (higherStatValue == basePoke.Attack)
        {
            higherStat = "attack";
        }
        else if (higherStatValue == basePoke.Defense)
        {
            higherStat = "defense";
        }
        else if (higherStatValue == basePoke.SpAttack)
        {
            higherStat = "spattack";
        }
        else if (higherStatValue == basePoke.SpDefense)
        {
            higherStat = "spdefense";
        }

        switch (higherStat)
        {
            case "hp":
                damageClass = "status";
                break;
            case "defense":
                damageClass = "status";
                break;
            case "spdefense":
                damageClass = "status";
                break;
            case "attack":
                damageClass = "physical";
                break;
            case "spattack":
                damageClass = "special";
                break;
        }

        return damageClass;
    }

    public bool TakeDamage(Move move, Pokemon attacker)
    {
        float modifiers = UnityEngine.Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        CurrentHP -= damage;

        if(currentHP <= 0)
        {
            currentHP = 0;
            return true;
        }
        return false;
    }

    public Move GetRandomMove()
    {
        int moveIndex = 0;
        do
        {
            moveIndex = UnityEngine.Random.Range(0, learntMoves.Length);
        } while (learntMoves[moveIndex] == null);
        return learntMoves[moveIndex];
    }
}
