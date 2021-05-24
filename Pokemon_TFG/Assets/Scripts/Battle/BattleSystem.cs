using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseMonster;

public class BattleSystem : MonoBehaviour
{
    public BattleUnit playerUnit;
    public BattleUnit enemyUnit;
    public BattleDialogBox dialogBox;
    public PartyScreen partyScreen;
    public BattleLoader battleLoader;

    private MonsterParty playerParty;
    private Monster wildMonster;

    private BattleState state;
    private int currentAction;
    private int currentMove;
    private int currentMember;

    private bool crActive = false;
    private bool playerFaster;

    private Coroutine actionRoutine;

    private IEnumerator CoroutineTypeText(string text)
    {
        crActive = true;
        yield return dialogBox.TypeDialog(text);
        crActive = false;
    }

    public enum BattleState{
        Start,
        ActionSelection, 
        MoveSelection,
        PartyScreen,
        PerformMove,
        Busy,
        BattleOver,
    }

    private void Start()
    {
        this.playerParty = Utils.monsterParty;
        this.wildMonster = Utils.wildMonster;
        StartCoroutine(SetupBattle());
    }

    private void Update()
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

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildMonster);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Monster.LearntMoves);

        yield return CoroutineTypeText("Ha aparecido un " + enemyUnit.Monster.BaseMonster.Name + " salvaje.");

        ChooseFirstTurn();
        ActionSelection();
    }

    private void ChooseFirstTurn()
    {
        if (playerUnit.Monster.Speed >= enemyUnit.Monster.Speed)
            playerFaster = true;
        else
            playerFaster = false;
    }

    private void BattleOver()
    {
        state = BattleState.BattleOver;
        playerParty.Monsters.ForEach(m => m.OnBattleOver());
        battleLoader.EndBattle();
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
        if(crActive)
        {
            StopCoroutine(actionRoutine);
            crActive = false;
        }

        state = BattleState.ActionSelection;

        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableDialogText(true);
        dialogBox.EnableActionSelector(true);

        actionRoutine = StartCoroutine(CoroutineTypeText("¿Que debería hacer " + playerUnit.Monster.BaseMonster.Name + "?"));

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
                    OpenPartyScreen();
                    break;

                case 3:
                    //Run
                    battleLoader.EndBattle();
                    break;
            }
        }
    }

    private void HandleMoveSelection()
    {
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
            if (currentMove == 0 || currentAction == 2)
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

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Monster.LearntMoves[currentMove]);

        if (Input.GetKeyDown(KeyCode.X))
        {
            ActionSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);

            dialogBox.EnableDialogText(true);

            if (playerFaster)
                StartCoroutine(PlayerMove());
            else
                StartCoroutine(EnemyMove(false));
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
            state = BattleState.PerformMove;
            StartCoroutine(SwitchMonster(selectedMonster));
        } else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableActionSelector(true);
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator SwitchMonster(Monster newMonster)
    {
        bool fainted = true;
        if(playerUnit.Monster.CurrentHP > 0)
        {
            fainted = false;
            yield return CoroutineTypeText($"¡{playerUnit.Monster.BaseMonster.Name}, vuelve aquí!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);
        }

        playerUnit.Monster.ResetStatBoosts();

        playerUnit.Setup(newMonster);

        dialogBox.SetMoveNames(newMonster.LearntMoves);

        yield return CoroutineTypeText("¡Adelante " + newMonster.BaseMonster.Name + "!");

        ChooseFirstTurn();

        if(!fainted)
        {
            StartCoroutine(EnemyMove(true));
        }
        else
        {
            ActionSelection();
        }

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

    private IEnumerator PlayerMove()
    {
        if (crActive)
        {
            StopCoroutine(actionRoutine);
            crActive = false;
        }

        state = BattleState.PerformMove;

        Move move = playerUnit.Monster.LearntMoves[currentMove];

        yield return RunMove(playerUnit, enemyUnit, move);
        
        if(state == BattleState.PerformMove)
        {
            if (playerFaster)
                yield return EnemyMove(true);
            else
                ActionSelection();
        }

    }

    private IEnumerator EnemyMove(bool playerAttackedAlready)
    {
        state = BattleState.PerformMove;

        Move move = enemyUnit.Monster.GetRandomMove();

        yield return RunMove(enemyUnit, playerUnit, move);

        if(state == BattleState.PerformMove)
        {
            if (playerFaster || (!playerFaster && playerAttackedAlready))
                ActionSelection();
            else if(!playerFaster && !playerAttackedAlready)
                yield return PlayerMove();
        }
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
            yield return dialogBox.TypeDialog(text);
        }
    }

    private void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.isPlayerUnit)
        {
            Monster nextPoke = playerParty.GetHealthyPokemon();
            if (nextPoke == null)
            {
                BattleOver();
            }
            else
            {
                OpenPartyScreen();
                
            }
        } else
        {
            BattleOver();
        }
    }

}
