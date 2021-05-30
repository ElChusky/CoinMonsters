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

    [SerializeField] int priority;

    private int currentPP;

    [SerializeField] MoveCategory moveCategory;

    [SerializeField] MoveEffects moveEffects;

    [SerializeField] List<SecondaryEffects> secondaryEffects;

    [SerializeField] MoveTarget target;

    public Move(Move move)
    {
        name = move.Name;
        description = move.Description;
        type = move.Type;
        power = move.Power;
        accuracy = move.accuracy;
        basePP = move.BasePp;
        currentPP = basePP;
        moveCategory = move._MoveCategory;
    }

    public Move(SavableMoveData savedData)
    {
        //We obtain the saved Move
        Move move = MoveDB.GetMoveByName(savedData.name);
        //I aisng the stored values from the saved Move to this move
        SetMoveData(move);
        //I overwrite the currentPP (which is asigned in previous method with the BasePP value) with the PP from the saved Move
        CurrentPP = savedData.pp;
    }

    public SavableMoveData GetSavedData()
    {
        SavableMoveData savedData = new SavableMoveData()
        {
            name = Name,
            pp = CurrentPP
        };
        return savedData;
    }

    public void SetMoveData(Move move)
    {
        name = move.Name;
        description = move.Description;
        type = move.Type;
        power = move.Power;
        accuracy = move.accuracy;
        basePP = move.BasePp;
        currentPP = basePP;
        moveCategory = move._MoveCategory;
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
    public int Priority { get { return priority; } set => priority = value; }
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
    Físico,
    Especial,
    Estado,
    Ninguno
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

[System.Serializable]
public class SavableMoveData
{
    public string name;
    public int pp;
}
