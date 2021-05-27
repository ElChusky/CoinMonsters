using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{

    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionID = kvp.Key;
            var condition = kvp.Value;

            condition.ID = conditionID;
        }
    }

    public static Dictionary<ConditionID, StatusCondition> Conditions { get; set; } = new Dictionary<ConditionID, StatusCondition>()
    {
        {
            ConditionID.psn,
            new StatusCondition(){
                Name = "Poison", StartMessage= "ha sido envenenado.", OnAfterTurn = (BattleUnit monsterUnit) =>
                {
                    monsterUnit.Monster.UpdateHP(monsterUnit.Monster.MaxHp / 8);
                    monsterUnit.Monster.StatusChanges.Enqueue((monsterUnit.isPlayerUnit) ? $"El veneno resta PS a {monsterUnit.Monster.BaseMonster.Name}." :
                        $"El veneno resta PS al {monsterUnit.Monster.BaseMonster.Name} enemigo.");
                }
            }
        },
        {
            ConditionID.brn,
            new StatusCondition(){
                Name = "Burn", StartMessage= "se ha quemado.", OnAfterTurn = (BattleUnit monsterUnit) =>
                {
                    monsterUnit.Monster.UpdateHP(monsterUnit.Monster.MaxHp / 16);
                    monsterUnit.Monster.StatusChanges.Enqueue((monsterUnit.isPlayerUnit) ? $"{monsterUnit.BaseMonster.Name} se resiente a las quemaduras." :
                        $"El {monsterUnit.BaseMonster.Name} enemigo se resiente a las quemaduras.");
                }
            }
        },
        {
            ConditionID.par,
            new StatusCondition(){
                Name = "Paralisis", StartMessage= "ha sido paralizado.", OnBeforeMove = (BattleUnit monsterUnit) =>
                {
                    if(Random.Range(1, 5) == 1)
                    {
                        monsterUnit.Monster.StatusChanges.Enqueue((monsterUnit.isPlayerUnit) ? $"{monsterUnit.BaseMonster.Name} está paralizado." :
                            $"El {monsterUnit.BaseMonster.Name} enemigo está paralizado.");
                        monsterUnit.Monster.StatusChanges.Enqueue("¡No se puede mover!");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new StatusCondition(){
                Name = "Freeze", StartMessage= "ha sido congelado.", OnBeforeMove = (BattleUnit monsterUnit) =>
                {
                    if(Random.Range(1, 5) == 1)
                    {
                        monsterUnit.Monster.CureStatus();
                        monsterUnit.Monster.StatusChanges.Enqueue((monsterUnit.isPlayerUnit) ? $"{monsterUnit.BaseMonster.Name} ya no está congelado." :
                            $"El {monsterUnit.BaseMonster.Name} enemigo ya no está congelado.");
                        return true;
                    }
                    monsterUnit.Monster.StatusChanges.Enqueue((monsterUnit.isPlayerUnit) ? $"{monsterUnit.BaseMonster.Name} está congelado." :
                        $"El {monsterUnit.BaseMonster.Name} enemigo está congelado.");
                    monsterUnit.Monster.StatusChanges.Enqueue("¡No se puede mover!");

                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new StatusCondition(){
                Name = "Sleep", StartMessage= "se ha dormido.", OnStart = (BattleUnit monsterUnit) =>
                {
                    //Sleep for 1-3 turns
                    monsterUnit.Monster.StatusTime = Random.Range(1, 4);
                },
                OnBeforeMove = (BattleUnit monsterUnit) =>
                {
                    if(monsterUnit.Monster.StatusTime <= 0)
                    {
                        monsterUnit.Monster.CureStatus();
                        monsterUnit.Monster.StatusChanges.Enqueue((monsterUnit.isPlayerUnit) ? $"¡{monsterUnit.BaseMonster.Name} se ha despertado!" :
                            $"¡El {monsterUnit.BaseMonster.Name} enemigo se ha despertado!");
                        return true;
                    }
                    monsterUnit.Monster.StatusTime--;
                    monsterUnit.Monster.StatusChanges.Enqueue((monsterUnit.isPlayerUnit) ? $"{monsterUnit.BaseMonster.Name} está durmiendo..." :
                        $"El {monsterUnit.BaseMonster.Name} enemigo está durmiendo...");
                    return false;
                }
            }
        },
        //Volatile Status Condition
        {
            ConditionID.confusion,
            new StatusCondition(){
                Name = "Confusion", StartMessage= "ha sido confundido.", OnStart = (BattleUnit monsterUnit) =>
                {
                    //Confused for 1-4 turns
                    monsterUnit.Monster.VolatileStatusTime = Random.Range(1, 5);
                },
                OnBeforeMove = (BattleUnit monsterUnit) =>
                {
                    if(monsterUnit.Monster.VolatileStatusTime <= 0)
                    {
                        monsterUnit.Monster.CureVolatileStatus();
                        monsterUnit.Monster.StatusChanges.Enqueue((monsterUnit.isPlayerUnit) ? $"¡{monsterUnit.BaseMonster.Name} ya no está confuso!" :
                            $"¡El {monsterUnit.BaseMonster.Name} enemigo ya no está confuso!");
                        return true;
                    }
                    monsterUnit.Monster.VolatileStatusTime--;
                    monsterUnit.Monster.StatusChanges.Enqueue((monsterUnit.isPlayerUnit) ? $"{monsterUnit.BaseMonster.Name} está confuso..." :
                        $"El {monsterUnit.BaseMonster.Name} enemigo está confuso...");

                    //50% chance of doing the move
                    if(Random.Range(1, 3) == 1)
                        return true;
                    
                    //Hurt by confusion
                    monsterUnit.Monster.StatusChanges.Enqueue("¡Está tan confuso que se hirió a si mismo!");

                    int damage = Mathf.FloorToInt((((2*monsterUnit.Monster.Level/5 + 2) * monsterUnit.Monster.Attack * 40)/monsterUnit.Monster.Defense) / 50 + 2);

                    monsterUnit.Monster.UpdateHP(damage);

                    return false;
                }
            }
        }
    };

    public static float GetStatusBonus(StatusCondition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.ID == ConditionID.slp || condition.ID == ConditionID.frz)
            return 2f;
        else if (condition.ID == ConditionID.par || condition.ID == ConditionID.psn || condition.ID == ConditionID.brn)
            return 1.5f;
        else
            return 1f;
    }
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz,
    confusion
}
