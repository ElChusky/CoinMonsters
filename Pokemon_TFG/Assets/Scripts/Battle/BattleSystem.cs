using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public BattleUnit playerUnit;
    public BattleHud playerHud;
    public BattleUnit enemyUnit;
    public BattleHud enemyHud;
    public BattleDialogBox dialogBox;

    private BattleState state;
    private int currentAction;
    private int currentMove;

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
        PlayerAction, 
        PlayerMove,
        EnemyMove,
        Busy,
    }

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        enemyUnit.Setup();
        enemyHud.SetData(enemyUnit.Pokemon);

        dialogBox.SetMoveNames(playerUnit.Pokemon.LearntMoves);

        yield return CoroutineTypeText("Ha aparecido un " + enemyUnit.Pokemon.BasePoke.Name + " salvaje.");

        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    private void PlayerMove()
    {
        state = BattleState.PlayerMove;

        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);

        dialogBox.EnableMoveSelector(true);
    }

    private void PlayerAction()
    {
        if(crActive)
        {
            StopCoroutine(actionRoutine);
            crActive = false;
        }

        state = BattleState.PlayerAction;

        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableDialogText(true);
        dialogBox.EnableActionSelector(true);

        actionRoutine = StartCoroutine(CoroutineTypeText("¿Que debería hacer " + playerUnit.Pokemon.BasePoke.Name + "?"));

        dialogBox.EnableActionSelector(true);
    }

    private void Update()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        } else if(state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(currentAction < 2)
                currentAction = currentAction + 2;
        } else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 1)
                currentAction = currentAction - 2;
        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(currentAction == 0 || currentAction == 2)
                currentAction++;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
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
                    PlayerMove();
                    break;

                case 1:
                    //Bag
                    break;

                case 2:
                    //Pokemon
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
                if(!dialogBox.moveTexts[currentMove + 2].text.Equals("-"))
                    currentMove += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
            {
                if(!dialogBox.moveTexts[currentMove - 2].text.Equals("-"))
                    currentMove -= 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove == 0 || currentAction == 2)
            {
                if(!dialogBox.moveTexts[currentMove + 1].text.Equals("-"))
                    currentMove++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove == 1 || currentAction == 3)
            {
                if(!dialogBox.moveTexts[currentMove - 1].text.Equals("-"))
                    currentMove--;
            }
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.LearntMoves[currentMove]);

        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayerAction();
        } else if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);

            dialogBox.EnableDialogText(true);

            StartCoroutine(PerformPlayerMove());
        }



    }

    private IEnumerator PerformPlayerMove()
    {
        if (crActive)
        {
            StopCoroutine(actionRoutine);
            crActive = false;
        }

        state = BattleState.Busy;

        Move move = playerUnit.Pokemon.LearntMoves[currentMove];

        yield return CoroutineTypeText($"{playerUnit.BasePokemon.Name} ha usado {move.Name}.");

        yield return new WaitForSeconds(1f);
        
        if (move.DamageClass.Equals("status"))
        {
            yield return EnemyMove();
        }
        else
        {
            bool fainted = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);

            yield return enemyHud.UpdateHP();

            if (fainted)
            {
                yield return CoroutineTypeText($"{enemyUnit.BasePokemon.Name} se ha debilitado.");
            }
            else
            {
                yield return EnemyMove();
            }
        }
    }

    private IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        Move move = enemyUnit.Pokemon.GetRandomMove();

        yield return CoroutineTypeText($"{enemyUnit.BasePokemon.Name} ha usado {move.Name}.");

        yield return new WaitForSeconds(1f);

        if (move.DamageClass.Equals("status"))
        {
            PlayerAction();
        } else
        {
            bool fainted = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);

            yield return playerHud.UpdateHP();

            if (fainted)
            {
                yield return CoroutineTypeText($"{playerUnit.BasePokemon.Name} se ha debilitado.");
            }
            else
            {
                PlayerAction();
            }
        }
    }

}
