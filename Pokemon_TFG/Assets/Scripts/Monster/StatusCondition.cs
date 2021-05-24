using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCondition
{
    public ConditionID ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    public Action<BattleUnit> OnAfterTurn { get; set; }
    public Func<BattleUnit, bool> OnBeforeMove { get; set; }
    public Action<BattleUnit> OnStart { get; set; }
}
