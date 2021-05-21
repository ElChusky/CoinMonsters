using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        StartBattle(Utils.monsterParty, Utils.wildMonster);
    }

    public void StartBattle(MonsterParty playerParty, Monster wildMonster)
    {
        this.playerParty = playerParty;
        this.wildMonster = wildMonster;
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

        ActionSelection();
    }

    private void BattleOver()
    {
        state = BattleState.BattleOver;
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

            StartCoroutine(PlayerMove());
        }

    }

    private void HandlePartyScreenSelection()
    {
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
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator SwitchMonster(Monster newMonster)
    {
        yield return dialogBox.TypeDialog($"¡{playerUnit.Monster.BaseMonster.Name}, vuelve aquí!");
        playerUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(1f);

        playerUnit.Setup(newMonster);

        dialogBox.SetMoveNames(newMonster.LearntMoves);

        yield return CoroutineTypeText("¡Adelante " + newMonster.BaseMonster.Name + "!");

        StartCoroutine(EnemyMove());
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
            yield return EnemyMove();

    }

    private IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        Move move = enemyUnit.Monster.GetRandomMove();

        yield return RunMove(enemyUnit, playerUnit, move);

        if(state == BattleState.PerformMove)
            ActionSelection();
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        if (sourceUnit.isPlayerUnit)
            yield return CoroutineTypeText($"¡{sourceUnit.BaseMonster.Name} usó {move.Name}!");
        else
            yield return CoroutineTypeText($"¡El {sourceUnit.BaseMonster.Name} enemigo usó {move.Name}!");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        move.CurrentPP--;

        targetUnit.PlayHitAnimation();

        DamageDetails damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
        yield return targetUnit.hud.UpdateHP();

        yield return ShowDamageDetails(damageDetails, targetUnit);

        if (damageDetails.Fainted)
        {
            targetUnit.PlayFaintAnimation();

            if (targetUnit.isPlayerUnit)
                yield return CoroutineTypeText($"¡{targetUnit.BaseMonster.Name} se debilitó!");
            else
                yield return CoroutineTypeText($"¡El {targetUnit.BaseMonster.Name} salvaje se debilitó!");

            CheckForBattleOver(targetUnit);
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
                faintedUnit.Setup(nextPoke);

                dialogBox.SetMoveNames(nextPoke.LearntMoves);

                StartCoroutine(CoroutineTypeText("¡Adelante " + nextPoke.BaseMonster.Name + "!"));

                ActionSelection();
            }
        } else
        {
            BattleOver();
        }
    }

}
