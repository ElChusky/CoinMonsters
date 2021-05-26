using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseMonster;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    PartyScreen,
    RunningTurn,
    Busy,
    BattleOver,
}

public enum BattleAction
{
    Move,
    SwitchMonster,
    UseItem,
    Run,
}

public class BattleSystem : MonoBehaviour
{
    public BattleUnit playerUnit;
    public BattleUnit enemyUnit;
    public BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    public PartyScreen partyScreen;

    private MonsterParty playerParty;
    private Monster wildMonster;

    private BattleState state;
    private BattleState? previousState;

    private int currentAction;
    private int currentMove;
    private int currentMember;

    private IEnumerator CoroutineTypeText(string text)
    {
        yield return dialogBox.TypeDialog(text);
        yield return new WaitUntil(() => dialogBox.dialogText.text.Equals(text));
        yield return new WaitForSeconds(0.5f);
    }

    public void StartBattle(MonsterParty monsterParty, Monster wildMonster)
    {
        this.playerParty = monsterParty;
        this.wildMonster = wildMonster;
        StartCoroutine(SetupBattle());
    }
    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildMonster);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Monster.LearntMoves);

        yield return CoroutineTypeText("Ha aparecido un " + enemyUnit.Monster.BaseMonster.Name + " salvaje.");

        ActionSelection();
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartyScreenSelection();
        }
    }


    private void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Monsters.ForEach(m => m.OnBattleOver());
        OnBattleOver(won);
    }

    private void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Monsters);
        partyScreen.gameObject.SetActive(true);
        partyScreen.UpdateMemberSelection(currentMember);
    }

    private void MoveSelection()
    {
        state = BattleState.MoveSelection;

        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);

        dialogBox.EnableMoveSelector(true);
    }

    private void ActionSelection()
    {
        state = BattleState.ActionSelection;

        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableDialogText(true);
        dialogBox.EnableActionSelector(true);

        StartCoroutine(CoroutineTypeText("¿Que debería hacer " + playerUnit.Monster.BaseMonster.Name + "?"));

        dialogBox.EnableActionSelector(true);
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 2)
                currentAction = currentAction + 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 1)
                currentAction = currentAction - 2;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentAction == 0 || currentAction == 2)
                currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentAction == 1 || currentAction == 3)
                currentAction--;
        }

        dialogBox.UdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            switch (currentAction)
            {
                case 0:
                    //Fight
                    MoveSelection();
                    break;

                case 1:
                    //Bag
                    break;

                case 2:
                    //Pokemon
                    previousState = state;
                    OpenPartyScreen();
                    break;

                case 3:
                    //Run
                    OnBattleOver(true);
                    break;
            }
        }
    }

    private void HandleMoveSelection()
    {
        #region Cambiar Movimiento
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < 2)
            {
                if (!dialogBox.moveTexts[currentMove + 2].text.Equals("-"))
                    currentMove += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
            {
                if (!dialogBox.moveTexts[currentMove - 2].text.Equals("-"))
                    currentMove -= 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove == 0 || currentMove == 2)
            {
                if (!dialogBox.moveTexts[currentMove + 1].text.Equals("-"))
                    currentMove++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove == 1 || currentAction == 3)
            {
                if (!dialogBox.moveTexts[currentMove - 1].text.Equals("-"))
                    currentMove--;
            }
        }
        #endregion

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Monster.LearntMoves[currentMove]);

        if (Input.GetKeyDown(KeyCode.X))
        {
            ActionSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {

            Move move = playerUnit.Monster.LearntMoves[currentMove];

            if (move.CurrentPP == 0) return;

            dialogBox.EnableMoveSelector(false);

            dialogBox.EnableDialogText(true);

            StartCoroutine(RunTurns(BattleAction.Move));
        }

    }

    private void HandlePartyScreenSelection()
    {
        dialogBox.EnableActionSelector(false);
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 2;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMember++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMember--;
        }

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Monsters.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Monster selectedMonster = playerParty.Monsters[currentMember];
            if (selectedMonster.CurrentHP <= 0)
            {
                partyScreen.SetMessageText("No puedes enviar a un monstruo debilidado.");
                return;
            }
            if(selectedMonster == playerUnit.Monster)
            {
                partyScreen.SetMessageText("Ese monstruo ya está luchando.");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            //If player chose to switch monsters, he was on ActionSelection state
            if(previousState == BattleState.ActionSelection)
            {
                previousState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchMonster));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchMonster(selectedMonster));
            }


        } else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableActionSelector(true);
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator SwitchMonster(Monster newMonster)
    {
        if(playerUnit.Monster.CurrentHP > 0)
        {
            yield return CoroutineTypeText($"¡{playerUnit.Monster.BaseMonster.Name}, vuelve aquí!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);
        }

        playerUnit.Monster.ResetStatBoosts();

        playerUnit.Setup(newMonster);

        dialogBox.SetMoveNames(newMonster.LearntMoves);

        yield return CoroutineTypeText("¡Adelante " + newMonster.BaseMonster.Name + "!");

        state = BattleState.RunningTurn;

    }

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails, BattleUnit pokemon)
    {

        if (damageDetails.Critical > 1f)
        {
            yield return CoroutineTypeText("¡Un golpe crítico!");
        }
        if(damageDetails.TypeEffectiveness > 1)
        {
            yield return CoroutineTypeText("¡Es muy eficaz!");
        } else if(damageDetails.TypeEffectiveness < 1 && damageDetails.TypeEffectiveness > 0)
        {
            yield return CoroutineTypeText("No es muy eficaz...");
        } else if(damageDetails.TypeEffectiveness == 0f)
        {
            yield return CoroutineTypeText($"No afecta al {pokemon.BaseMonster.Name} enemigo.");
        }
    }

    private IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;
        if(playerAction == BattleAction.Move)
        {

            playerUnit.Monster.CurrentMove = playerUnit.Monster.LearntMoves[currentMove];
            enemyUnit.Monster.CurrentMove = enemyUnit.Monster.GetRandomMove();

            //Check order
            int playerMovePriority = playerUnit.Monster.CurrentMove.Priority;
            int enemyMovePriority = enemyUnit.Monster.CurrentMove.Priority;

            bool playerGoesFirst = true;

            if(enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if(enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Monster.Speed >= enemyUnit.Monster.Speed;

            BattleUnit firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            BattleUnit secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            Monster secondMonster = secondUnit.Monster;

            //First unit run move
            yield return RunMove(firstUnit, secondUnit, firstUnit.Monster.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if(secondMonster.CurrentHP > 0)
            {
                //Second unit run move
                yield return RunMove(secondUnit, firstUnit, secondUnit.Monster.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }

        }
        else if(playerAction == BattleAction.SwitchMonster)
        {
            //Switch Monster
            Monster selectedMonster = playerParty.Monsters[currentMember];
            state = BattleState.Busy;
            yield return SwitchMonster(selectedMonster);

            //Enemy gets turn
            Move enemyMove = enemyUnit.Monster.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;

        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        //If Monster cant move due to a status condition, it wont execute the rest of code
        int hpBeforeDamagedByStatus = sourceUnit.Monster.CurrentHP;
        bool canMove = sourceUnit.Monster.OnBeforeMove(sourceUnit);
        int hpAfterDamagedByStatus = sourceUnit.Monster.CurrentHP;

        if (!canMove)
        {
            yield return ShowStatusChanges(sourceUnit.Monster);
            if(hpBeforeDamagedByStatus > hpAfterDamagedByStatus)
                sourceUnit.PlayHitAnimation();
            yield return sourceUnit.hud.UpdateHP();
            yield break;
        }
        
        yield return ShowStatusChanges(sourceUnit.Monster);

        if (sourceUnit.isPlayerUnit)
            yield return CoroutineTypeText($"¡{sourceUnit.BaseMonster.Name} usó {move.Name}!");
        else
            yield return CoroutineTypeText($"¡El {sourceUnit.BaseMonster.Name} enemigo usó {move.Name}!");

        if (CheckIfMoveHits(move, sourceUnit.Monster, targetUnit.Monster))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            move.CurrentPP--;

            if (move._MoveCategory == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.MoveEffects, sourceUnit, targetUnit, move.Target);
            }
            else
            {
                targetUnit.PlayHitAnimation();

                DamageDetails damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
                yield return targetUnit.hud.UpdateHP();
                yield return sourceUnit.hud.UpdateHP();

                yield return ShowDamageDetails(damageDetails, targetUnit);
            }

            if(move.SecondaryEffects != null && move.SecondaryEffects.Count > 0 && targetUnit.Monster.CurrentHP > 0)
            {
                foreach (SecondaryEffects secEffects in move.SecondaryEffects)
                {
                    if (UnityEngine.Random.Range(1, 101) <= secEffects.Chance)
                        yield return RunMoveEffects(secEffects, sourceUnit, targetUnit, secEffects.Target);
                }
            }

            if (targetUnit.Monster.CurrentHP <= 0)
            {
                targetUnit.PlayFaintAnimation();

                if (targetUnit.isPlayerUnit)
                    yield return CoroutineTypeText($"¡{targetUnit.BaseMonster.Name} se debilitó!");
                else
                    yield return CoroutineTypeText($"¡El {targetUnit.BaseMonster.Name} salvaje se debilitó!");

                CheckForBattleOver(targetUnit);
            }
        }
        else
        {
            if (sourceUnit.isPlayerUnit)
                yield return CoroutineTypeText($"El ataque de {sourceUnit.BaseMonster.Name} falló.");
            else
                yield return CoroutineTypeText($"El ataque del {sourceUnit.BaseMonster.Name} salvaje falló.");

        }

    }

    private IEnumerator RunMoveEffects(MoveEffects effects, BattleUnit sourceUnit, BattleUnit targetUnit, MoveTarget target)
    {
        //Stat Boosting
        if (effects.Boosts != null)
        {
            sourceUnit.Monster.ApplyBoosts(effects.Boosts, sourceUnit, targetUnit, target);
        }

        //Status Conditions
        if(effects.Status != ConditionID.none)
        {
            targetUnit.Monster.SetStatus(effects.Status, targetUnit);
        }

        //VolatileStatus Conditions
        if (effects.VolatileStatus != ConditionID.none)
        {
            targetUnit.Monster.SetVolatileStatus(effects.VolatileStatus, targetUnit);
        }

        yield return ShowStatusChanges(sourceUnit.Monster);

        yield return ShowStatusChanges(targetUnit.Monster);
    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {

        if (state == BattleState.BattleOver) yield break;

        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        //Statuses like burn or poison will hurt the monster at the end of the turn
        sourceUnit.Monster.OnAfterTurn(sourceUnit);

        yield return ShowStatusChanges(sourceUnit.Monster);
        yield return sourceUnit.hud.UpdateHP();

        if (sourceUnit.Monster.CurrentHP <= 0)
        {
            sourceUnit.PlayFaintAnimation();

            if (sourceUnit.isPlayerUnit)
                yield return CoroutineTypeText($"¡{sourceUnit.BaseMonster.Name} se debilitó!");
            else
                yield return CoroutineTypeText($"¡El {sourceUnit.BaseMonster.Name} salvaje se debilitó!");

            CheckForBattleOver(sourceUnit);
        }
    }

    private bool CheckIfMoveHits(Move move, Monster sourceMonster, Monster targetMonster)
    {
        if (move.AlwaysHit)
            return true;

        float moveAccuracy = move.Accuracy;

        int accuracy = sourceMonster.StatBoosts[Stat.Accuracy];
        int evasion = targetMonster.StatBoosts[Stat.Evasion];


        float[] boostValues = new float[] { 1f, 4f/3f, 5f/3f, 2f, 7f/3f, 8f/3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    private IEnumerator ShowStatusChanges(Monster monster)
    {
        while(monster.StatusChanges != null && monster.StatusChanges.Count > 0)
        {
            string text = monster.StatusChanges.Dequeue();
            yield return CoroutineTypeText(text);
        }
    }

    private void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.isPlayerUnit)
        {
            Monster nextPoke = playerParty.GetHealthyPokemon();
            if (nextPoke == null)
            {
                BattleOver(false);
            }
            else
            {
                OpenPartyScreen();
                
            }
        } else
        {
            BattleOver(true);
        }
    }

}
