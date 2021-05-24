using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseMonster;

[CreateAssetMenu(fileName = "Move", menuName = "Move/Create new Move")]
public class Move : ScriptableObject
{
    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] MonsterType type;

    [SerializeField] int power;

    [SerializeField] int accuracy;

    [SerializeField] bool alwaysHit;

    [SerializeField] int basePP;

    private int currentPP;

    [SerializeField] MoveCategory moveCategory;

    [SerializeField] MoveEffects moveEffects;

    [SerializeField] List<SecondaryEffects> secondaryEffects;

    [SerializeField] MoveTarget target;


    public Move(string name, string description, MonsterType type, int power, int accuracy, int basePP, MoveCategory moveCategory)
    {
        this.name = name;
        this.description = description;
        this.type = type;
        this.power = power;
        this.accuracy = accuracy;
        this.basePP = basePP;
        this.currentPP = basePP;
        this.moveCategory = moveCategory;
    }

    public string Name { get { return name; } set => name = value; }
    public string Description { get { return description; } set => description = value; }
    public MonsterType Type { get { return type; } set => type = value; }
    public int Power { get { return power; } set => power = value; }
    public int Accuracy { get { return accuracy; } set => accuracy = value; }
    public bool AlwaysHit { get { return alwaysHit; } set => alwaysHit = value; }
    public int BasePp { get { return basePP; } set => basePP = value; }
    public MoveCategory _MoveCategory { get { return moveCategory; } set => moveCategory = value; }
    public int CurrentPP { get { return currentPP; } set => currentPP = value; }
    public MoveEffects MoveEffects { get { return moveEffects; } }
    public List<SecondaryEffects> SecondaryEffects { get { return secondaryEffects; } }
    public MoveTarget Target { get { return target; } }

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

[System.Serializable]
public class LearnableMove
{
    [SerializeField] Move move;
    [SerializeField] int level;

    public Move Move { get => move; set => move = value; }
    public int Level { get => level; set => level = value; }
}

public enum MoveCategory
{
    Physical,
    Special,
    Status
}

public enum MoveTarget
{
    Foe,
    Self
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;
    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }

    public ConditionID Status { get { return status; } }
    public ConditionID VolatileStatus { get { return volatileStatus; } }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance { get { return chance; } }
    public MoveTarget Target { get { return target; } }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}
