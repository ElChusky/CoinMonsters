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

        yield return dialogBox.TypeDialog("Ha aparecido un " + enemyUnit.Pokemon.BasePoke.Name + " salvaje.");

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
        state = BattleState.PlayerAction;
        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableDialogText(true);
        dialogBox.EnableActionSelector(true);
        StartCoroutine(dialogBox.TypeDialog("¿Que debería hacer " + playerUnit.Pokemon.BasePoke.Name + "?"));
        dialogBox.EnableActionSelector(true);
    }

    private void Update()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
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
        } else if (Input.GetKeyDown(KeyCode.X))
        {
            switch (state)
            {
                case BattleState.PlayerMove:
                    //Fight
                    PlayerAction();
                    break;
            }
        }
    }


}
