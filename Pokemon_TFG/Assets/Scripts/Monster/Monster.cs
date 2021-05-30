using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BaseMonster;

[System.Serializable]
public class Monster
{
    [SerializeField] BaseMonster baseMonster;
    [SerializeField] int level;
    private int currentHP;
    private int hpIV, attackIV, defenseIV, spAttackIV, spDefenseIV, speedIV;
    private List<Move> learntMoves;

    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }
    public Move CurrentMove { get; set; }
    public Condition VolatileStatus { get; set; }
    public int VolatileStatusTime { get; set; }
    public bool HpChanged { get; set; }

    public event Action OnStatusChanged;

    public Monster(BaseMonster baseMonster, int level)
    {
        this.baseMonster = baseMonster;
        this.level = level;

        Init();
    }

    public void Init()
    {
        learntMoves = new List<Move>();
        SetIVs();
        GiveLearntMoves();

        Exp = BaseMonster.GetExpForLevel(level);

        CalculateStats();
        currentHP = MaxHp;

        ResetStatBoosts();
        Status = null;
        VolatileStatus = null;
        StatusChanges = new Queue<string>();
    }

    public Monster(SavableMonsterData savedData)
    {
        BaseMonster = MonstersDB.GetMonsterByName(savedData.name);

        CurrentHP = savedData.currentHP;
        Level = savedData.level;
        Exp = savedData.exp;

        HpIV = savedData.hpIV;
        AttackIV = savedData.attackIV;
        DefenseIV = savedData.defenseIV;
        SpAttackIV = savedData.spAttackIV;
        SpDefenseIV = savedData.spDefenseIV;
        SpeedIV = savedData.speedIV;

        if (savedData.statusID != null)
            Status = ConditionsDB.Conditions[savedData.statusID.Value];
        else
            Status = null;

        LearntMoves = savedData.moves.Select(s => new Move(s)).ToList();

        CalculateStats();
        ResetStatBoosts();
        VolatileStatus = null;
        StatusChanges = new Queue<string>();
    }

    public SavableMonsterData GetSavedData()
    {
        SavableMonsterData savedData = new SavableMonsterData()
        {
            name = baseMonster.Name,
            currentHP = CurrentHP,
            level = Level,
            hpIV = HpIV,
            attackIV = AttackIV,
            defenseIV = HpIV,
            spAttackIV = SpAttackIV,
            spDefenseIV = SpDefenseIV,
            speedIV = SpeedIV,
            exp = Exp,
            statusID = Status?.ID,
            moves = LearntMoves.Select(m => m.GetSavedData()).ToList()
        };
        return savedData;
    }

    private void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt(((2 * BaseMonster.Attack + AttackIV) * Level * 0.01f) + 5));
        Stats.Add(Stat.Defense, Mathf.FloorToInt(((2 * BaseMonster.Defense + DefenseIV) * Level * 0.01f) + 5));
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt(((2 * BaseMonster.SpAttack + SpAttackIV) * Level * 0.01f) + 5));
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt(((2 * BaseMonster.SpDefense + SpDefenseIV) * Level * 0.01f) + 5));
        Stats.Add(Stat.Speed, Mathf.FloorToInt(((2 * BaseMonster.Speed + SpeedIV) * Level * 0.01f) + 5));
        MaxHp = Mathf.FloorToInt(((2 * BaseMonster.MaxHp + HpIV) * Level * 0.01f) + Level + 10);
    }

    private int GetStat(Stat stat)
    {
        int statValue = Stats[stat];

        //Apply stat boost
        int boost = StatBoosts[stat];
        float[] boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statValue = Mathf.FloorToInt(statValue * boostValues[boost]);
        else
            statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);

        return statValue;
    }

    public void ResetStatBoosts()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},

            {Stat.Accuracy, 0},
            {Stat.Evasion, 0},
        };
    }

    public void ApplyBoosts(List<StatBoost> boosts, BattleUnit sourceUnit, BattleUnit targetUnit, MoveTarget target)
    {
        foreach (StatBoost statBoost in boosts)
        {
            Stat stat = statBoost.stat;
            int boost = statBoost.boost;

            if (stat == Stat.HP)
                continue;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            string text;

            if ((sourceUnit.isPlayerUnit && target == MoveTarget.Foe) || (!sourceUnit.isPlayerUnit && target == MoveTarget.Self))
                text = $"del {targetUnit.BaseMonster.Name} enemigo";
            else if(sourceUnit.isPlayerUnit && target == MoveTarget.Self)
                text = $"de {sourceUnit.BaseMonster.Name}";
            else
                text = $"de {targetUnit.BaseMonster.Name}";

            if (statBoost.boost > 0)
                StatusChanges.Enqueue($"{GetStatusText(statBoost)}" + text + " ha aumentado.");
            else
                StatusChanges.Enqueue($"{GetStatusText(statBoost)}" + text + " ha disminuido.");

            Debug.Log($"{stat} ha cambiado a: {StatBoosts[stat]}");
        }
    }

    private string GetStatusText(StatBoost statBoost)
    {
        Stat statName = statBoost.stat;
        switch (statName)
        {
            case Stat.Attack:
                return "El ataque ";
            case Stat.Defense:
                return "La defensa ";
            case Stat.SpAttack:
                return "El ataque especial ";
            case Stat.SpDefense:
                return "La defensa especial ";
            case Stat.Speed:
                return "La velocidad ";
        }
        return "No va a llegar aqui";
    }

    #region Stats
    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxHp { get; private set; }
    #endregion

    public Queue<string> StatusChanges { get; private set; }

    public BaseMonster BaseMonster { get => baseMonster; set => baseMonster = value; }

    public int Exp { get; set; }

    public int Level { get => level; set => level = value; }

    public int CurrentHP { get => currentHP; set => currentHP = value; }

    public List<Move> LearntMoves { get => learntMoves; set => learntMoves = value; }

    #region Pokemon IV
    public int HpIV { get => hpIV; set => hpIV = value; }
    public int AttackIV { get => attackIV; set => attackIV = value; }
    public int DefenseIV { get => defenseIV; set => defenseIV = value; }
    public int SpAttackIV { get => spAttackIV; set => spAttackIV = value; }
    public int SpDefenseIV { get => spDefenseIV; set => spDefenseIV = value; }
    public int SpeedIV { get => speedIV; set => speedIV = value; }
    #endregion

    public bool CheckForLevelUp()
    {
        if(Exp > baseMonster.GetExpForLevel(Level + 1))
        {
            level++;
            return true;
        }

        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrentLevel()
    {
        return baseMonster.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnMove(LearnableMove movetoLearn)
    {
        if (LearntMoves.Count > 4)
            return;

        Move newMove = ScriptableObject.CreateInstance<Move>();
        newMove.SetMoveData(movetoLearn.Move);
        learntMoves.Add(newMove);
    }

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
        List<LearnableMove> moves = BaseMonster.LearnableMoves;
        
        List<Move> learnableMoves = new List<Move>();
        foreach (LearnableMove move in moves)
        {
            if (move.Level <= level)
            {
                learnableMoves.Add(move.Move);
            }
        }
        if (learnableMoves.Count <= 4)
        {
            for (int i = 0; i < learnableMoves.Count; i++)
            {
                learnableMoves[i].CurrentPP = learnableMoves[i].BasePp;
                learntMoves.Add(learnableMoves[i]);
            }
        }
        else
        {

            #region Codigo para dar 1 move de cada tipo, y cuando ya se hayan dado, dar en funcion de la stat mas alta hasta 4 moves si se puede. Comentar y descomentar el otro para cambiarlo

            bool tieneFisico = false, tieneEspecial = false, tieneStatus = false;
            //With this loop we give 1 attack of each type, physical dmg, special dmg and status move if there are any
            foreach (Move currentMove in learnableMoves)
            {
                if (currentMove._MoveCategory == MoveCategory.Físico && !tieneFisico)
                {
                    learntMoves.Add(currentMove);
                    tieneFisico = true;

                }
                else if (currentMove._MoveCategory == MoveCategory.Especial && !tieneEspecial)
                {
                    learntMoves.Add(currentMove);
                    tieneEspecial = true;
                }
                else if (currentMove._MoveCategory == MoveCategory.Estado && !tieneStatus)
                {
                    learntMoves.Add(currentMove);
                    tieneStatus = true;
                }
                if (tieneFisico && tieneEspecial && tieneStatus)
                {
                    break;
                }
            }
            //Now we will check which base stat of the pokemon is higher, so we choose to give another physical dmg move (attack higher),
            //another special dmg move (sp attack higher) or another status move (hp, defense or sp defense higher)
            MoveCategory prevDamageClass = MoveCategory.Ninguno;
            for (int higherStat = 0; higherStat < 5; higherStat++)
            {
                MoveCategory damageClass = MoveCategorySearch(higherStat);

                if(prevDamageClass != MoveCategory.Ninguno && damageClass == prevDamageClass)
                {
                    continue;
                }

                prevDamageClass = damageClass;

                for (int index = 0; index < learnableMoves.Count; index++)
                {
                    Move currentMove = learnableMoves[index];
                    if (currentMove._MoveCategory == damageClass)
                    {
                        bool learnt = false;
                        for (int i = 0; i < learntMoves.Count; i++)
                        {
                            if (currentMove.Equals(learntMoves[i]))
                            {
                                learnt = true;
                                break;
                            }
                        }
                        if (!learnt)
                        {
                            learntMoves.Add(currentMove);
                            break;
                        }
                    }
                }
                if (learntMoves.Count == 4)
                    break;
            }
            #endregion
            /*
            #region Dar los 4 movimientos de mayor nivel. Comentar y descomentar el otro para cambiarlo.
            //Reordena la lista en orden contrario al de entrada, es decir, por nivel mas alto. IMPORTANTE introducir los ataques ordenados en Unity
            learnableMoves.Reverse();
            foreach (Move move in learnableMoves)
            {
                if (learntMoves.Count < 4)
                    learntMoves.Add(move);
                else
                    break;
            }
            #endregion
            */
        }
    }

    private MoveCategory MoveCategorySearch(int highStatPos)
    {
        MoveCategory moveCategory = MoveCategory.Físico;
        string higherStat = "";
        int[] stats = new int[5];
        stats[0] = baseMonster.MaxHp;
        stats[1] = baseMonster.Attack;
        stats[2] = baseMonster.Defense;
        stats[3] = baseMonster.SpAttack;
        stats[4] = baseMonster.SpDefense;
        Array.Sort(stats);
        Array.Reverse(stats);
        int higherStatValue = stats[highStatPos];
        if (higherStatValue == baseMonster.MaxHp)
        {
            higherStat = "hp";
        }
        else if (higherStatValue == baseMonster.Attack)
        {
            higherStat = "attack";
        }
        else if (higherStatValue == baseMonster.Defense)
        {
            higherStat = "defense";
        }
        else if (higherStatValue == baseMonster.SpAttack)
        {
            higherStat = "spattack";
        }
        else if (higherStatValue == baseMonster.SpDefense)
        {
            higherStat = "spdefense";
        }

        switch (higherStat)
        {
            case "hp":
                moveCategory = MoveCategory.Estado;
                break;
            case "defense":
                moveCategory = MoveCategory.Estado;
                break;
            case "spdefense":
                moveCategory = MoveCategory.Estado;
                break;
            case "attack":
                moveCategory = MoveCategory.Físico;
                break;
            case "spattack":
                moveCategory = MoveCategory.Especial;
                break;
        }

        return moveCategory;
    }

    public DamageDetails TakeDamage(Move move, Monster attacker)
    {
        float criticalHit = 1f;

        if(UnityEngine.Random.value * 100 <= 6.25f)
        {
            criticalHit = 2f;
        }

        float typeMultiplier = TypeChart.GetEffectiveness(move.Type, this);

        DamageDetails damageDetails = new DamageDetails()
        {
            TypeEffectiveness = typeMultiplier,
            Critical = criticalHit,
            Fainted = false
        };

        //If attack is a special attack, it uses sp attack and sp defense to calculate the dmg, otherwise it uses the normal attack and defense
        float attack = (move._MoveCategory == MoveCategory.Especial) ? attacker.SpAttack : attacker.Attack; 
        float defense = (move._MoveCategory == MoveCategory.Especial) ? SpDefense : Defense;

        float modifiers = UnityEngine.Random.Range(0.85f, 1f) * typeMultiplier * criticalHit;

        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Power * ((float)attack / defense) + 2;

        int damage = Mathf.FloorToInt(d * modifiers);

        if (CurrentHP < damage)
            damage = CurrentHP;

        UpdateHP(damage);

        if(move.SecondaryEffects != null)
        {
            foreach (SecondaryEffects secEffect in move.SecondaryEffects)
            {
                if (secEffect.Boosts != null)
                {
                    foreach (StatBoost statBoost in secEffect.Boosts)
                    {
                        if (statBoost.stat == Stat.HP)
                        {
                            if (damage == 1)
                                damage++;
                            attacker.UpdateHP(-damage * statBoost.boost/100);
                            StatusChanges.Enqueue($"{attacker.BaseMonster.Name} ha recuperado vida.");
                            break;
                        }
                    }
                    break;
                }
            }
        }

        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        currentHP = Mathf.Clamp(CurrentHP - damage, 0, MaxHp);
        HpChanged = true;
    }

    public void SetStatus(ConditionID conditionId, BattleUnit monsterUnit)
    {
        if (Status != null)
        {
            StatusChanges.Enqueue("Pero falló.");
            return;
        }
        Status = ConditionsDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(monsterUnit);
        StatusChanges.Enqueue($"{BaseMonster.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionId, BattleUnit monsterUnit)
    {
        if (VolatileStatus != null)
        {
            StatusChanges.Enqueue("Pero falló.");
            return;
        }
        VolatileStatus = ConditionsDB.Conditions[conditionId];
        VolatileStatus?.OnStart?.Invoke(monsterUnit);
        StatusChanges.Enqueue($"{BaseMonster.Name} {VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {

        List<Move> movesWithPP = new List<Move>();

        foreach (Move move in learntMoves)
        {
            if (move != null && move.CurrentPP > 0)
                movesWithPP.Add(move);
        }

        int moveIndex = UnityEngine.Random.Range(0, movesWithPP.Count);
        return movesWithPP[moveIndex];
    }

    public void OnAfterTurn(BattleUnit monsterUnit)
    {
        Status?.OnAfterTurn?.Invoke(monsterUnit);
        VolatileStatus?.OnAfterTurn?.Invoke(monsterUnit);
    }

    public bool OnBeforeMove(BattleUnit monsterUnit)
    {
        bool canPerformMove = true;
        if(Status?.OnBeforeMove != null)
        {
            if(!Status.OnBeforeMove(monsterUnit))
                canPerformMove = false;
        }

        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(monsterUnit))
                canPerformMove = false;
        }

        return canPerformMove;
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoosts();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}

[System.Serializable]
public class SavableMonsterData
{
    public string name;
    public int currentHP;
    public int level;
    public int hpIV, attackIV, defenseIV, spAttackIV, spDefenseIV, speedIV;
    public int exp;
    public ConditionID? statusID;
    public List<SavableMoveData> moves;
}
