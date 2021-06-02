using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    AboutToUSe,
    MoveToForget,
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
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject monsterBall;
    [SerializeField] MoveSelectionUI moveSelectionUI;

    public event Action<bool> OnBattleOver;

    private BattleState state;
    private BattleState? previousState;

    private int currentAction;
    private int currentMove;
    private int currentMember;
    private int currentMoveToForgetSelection;

    private int escapeAttempts;
    private Move moveToLearn;
    private bool aboutToUse;

    private MonsterParty playerParty;
    private MonsterParty trainerParty;
    private Monster wildMonster;
    private Fader fader;

    private bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    private IEnumerator CoroutineTypeText(string text, float time = 0.5f)
    {
        yield return dialogBox.TypeDialog(text);
        yield return new WaitForSeconds(time);
    }

    public void StartBattle(MonsterParty monsterParty, Monster wildMonster)
    {
        currentAction = 0;
        currentMember = 0;
        currentMove = 0;
        aboutToUse = true;

        isTrainerBattle = false;

        player = monsterParty.GetComponent<PlayerController>();
        this.playerParty = monsterParty;
        this.wildMonster = wildMonster;
        StartCoroutine(SetupBattle());
    }

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    public void StartTrainerBattle(MonsterParty monsterParty, MonsterParty trainerParty)
    {
        currentAction = 0;
        currentMember = 0;
        currentMove = 0;
        aboutToUse = true;

        this.playerParty = monsterParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isTrainerBattle)
        {
            //Wild Monster Battle
            playerUnit.Setup(playerParty.GetHealthyPokemon());
            enemyUnit.Setup(wildMonster);

            dialogBox.SetMoveNames(playerUnit.Monster.LearntMoves);

            yield return CoroutineTypeText("Ha aparecido un " + enemyUnit.Monster.BaseMonster.Name + " salvaje.");
        }
        else
        {
            //Trainer Battle

            //Show trainer and player sprites
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return CoroutineTypeText($"¡{trainer.Name} quiere pelear!");

            //Send out first trainer monster
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            Monster enemyMonster = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyMonster);
            yield return CoroutineTypeText($"¡{trainer.Name} ha enviado a {enemyMonster.BaseMonster.Name}!");

            //Send out first player monster
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            Monster playerMonster = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerMonster);

            dialogBox.SetMoveNames(playerUnit.Monster.LearntMoves);
            yield return CoroutineTypeText($"¡Adelante, {playerMonster.BaseMonster.Name}!");

        }

        yield return CoroutineTypeText("¿Que debería hacer " + playerUnit.Monster.BaseMonster.Name + "?", 0f);

        escapeAttempts = 0;
        partyScreen.Init();

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
        } else if(state == BattleState.AboutToUSe)
        {
            HandleAboutToUse();
        } else if(state == BattleState.MoveToForget)
        {
            HandleMoveToForgetSelection();
        }
    }

    private void BattleOver(bool won)
    {
        state = BattleState.BattleOver;

        playerParty.Monsters.ForEach(m => m.OnBattleOver());

        dialogBox.EnableActionSelector(false);

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
    }

    private IEnumerator AboutToUse(Monster newMonster)
    {
        state = BattleState.Busy;
        yield return CoroutineTypeText($"{trainer.Name} va a sacar a {newMonster.BaseMonster.Name}. ¿Quieres cambiar de Monstruo?");

        state = BattleState.AboutToUSe;
        dialogBox.EnableChoiceBox(true);
    }

    private IEnumerator ChooseMoveToForget(Monster monster, Move newMove)
    {
        state = BattleState.Busy;
        yield return CoroutineTypeText($"¿Quieres olvidar un movimiento?");

        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(monster.LearntMoves, newMove);
        moveToLearn = newMove;

        state = BattleState.MoveToForget;
    }

    private void HandleMoveToForgetSelection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMoveToForgetSelection--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMoveToForgetSelection++;

        currentMoveToForgetSelection = Mathf.Clamp(currentMoveToForgetSelection, 0, 4);

        moveSelectionUI.UpdateMoveSelection(currentMoveToForgetSelection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            moveSelectionUI.gameObject.SetActive(false);
            if(currentMoveToForgetSelection == 4)
            {
                //Dont learn new Move
                StartCoroutine(CoroutineTypeText($"{playerUnit.Monster.BaseMonster.Name} no aprendió {moveToLearn.Name}"));
            }
            else
            {
                //Forget selected Move and learn the new one
                Move moveToForget = playerUnit.Monster.LearntMoves[currentMoveToForgetSelection];

                StartCoroutine(CoroutineTypeText($"{playerUnit.Monster.BaseMonster.Name} ha olvidado {moveToForget.Name} y ha aprendido {moveToLearn.Name}"));

                Move toLearnMove = ScriptableObject.CreateInstance<Move>();
                toLearnMove.SetMoveData(moveToLearn);
                playerUnit.Monster.LearntMoves[currentMoveToForgetSelection] = toLearnMove;
            }

            moveToLearn = null;
            state = BattleState.RunningTurn;
        }
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 2)
                currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 1)
                currentAction -= 2;
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
                    StartCoroutine(RunTurns(BattleAction.UseItem));
                    break;

                case 2:
                    //Pokemon
                    previousState = state;
                    OpenPartyScreen();
                    break;

                case 3:
                    //Run
                    StartCoroutine(RunTurns(BattleAction.Run));
                    break;
            }
            dialogBox.EnableActionSelector(false);
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
            if (currentMove == 1 || currentMove == 3)
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
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            currentMember++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentMember--;

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
            if(playerUnit.Monster.CurrentHP <= 0)
            {
                partyScreen.SetMessageText("Tienes que escoger un Monstruo para continuar.");
                return;
            }

            dialogBox.EnableActionSelector(true);
            partyScreen.gameObject.SetActive(false);

            if (previousState == BattleState.AboutToUSe)
            {
                previousState = null;
                StartCoroutine(SendNextTrainerMonster());
            }
            else
            {
                StartCoroutine(CoroutineTypeText("¿Que debería hacer " + playerUnit.Monster.BaseMonster.Name + "?", 0f));

                ActionSelection();
            }
        }
    }

    private void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            aboutToUse = !aboutToUse;

        dialogBox.UpdateChoiceSelection(aboutToUse);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableChoiceBox(false);
            if (aboutToUse)
            {
                //Yes option
                previousState = BattleState.AboutToUSe;
                OpenPartyScreen();
            }
            else
            {
                //No option
                StartCoroutine(SendNextTrainerMonster());
            }
        } else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerMonster());
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

        if(previousState == null)
            state = BattleState.RunningTurn;
        else if(previousState == BattleState.AboutToUSe)
        {
            previousState = null;
            StartCoroutine(SendNextTrainerMonster());
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

    private IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;
        if (playerAction == BattleAction.Move)
        {

            playerUnit.Monster.CurrentMove = playerUnit.Monster.LearntMoves[currentMove];
            enemyUnit.Monster.CurrentMove = enemyUnit.Monster.GetRandomMove();

            //Check order
            int playerMovePriority = playerUnit.Monster.CurrentMove.Priority;
            int enemyMovePriority = enemyUnit.Monster.CurrentMove.Priority;

            bool playerGoesFirst = true;

            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Monster.Speed >= enemyUnit.Monster.Speed;

            BattleUnit firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            BattleUnit secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            Monster secondMonster = secondUnit.Monster;

            //First unit run move
            yield return RunMove(firstUnit, secondUnit, firstUnit.Monster.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondMonster.CurrentHP > 0)
            {
                //Second unit run move
                yield return RunMove(secondUnit, firstUnit, secondUnit.Monster.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }

        }
        else
        {
            if (playerAction == BattleAction.SwitchMonster)
            {
                //Switch Monster
                Monster selectedMonster = playerParty.Monsters[currentMember];
                state = BattleState.Busy;
                yield return SwitchMonster(selectedMonster);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                dialogBox.EnableActionSelector(false);
                yield return ThrowMonsterBall();
            } else if (playerAction == BattleAction.Run)
                yield return TryToEscape();

            //Enemy gets turn
            Move enemyMove = enemyUnit.Monster.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }
        if (state != BattleState.BattleOver)
        {
            yield return CoroutineTypeText("¿Que debería hacer " + playerUnit.Monster.BaseMonster.Name + "?", 0f);
            ActionSelection();
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

            if (move._MoveCategory == MoveCategory.Estado)
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
                yield return HandleMonsterFainted(targetUnit);
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
            yield return HandleMonsterFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
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

    private IEnumerator HandleMonsterFainted(BattleUnit faintedUnit)
    {
        faintedUnit.PlayFaintAnimation();

        if (faintedUnit.isPlayerUnit)
            yield return CoroutineTypeText($"¡{faintedUnit.BaseMonster.Name} se debilitó!");
        else
            yield return CoroutineTypeText($"¡El {faintedUnit.BaseMonster.Name} salvaje se debilitó!");

        if (!faintedUnit.isPlayerUnit)
        {
            //Exp Gain
            int expYield = faintedUnit.BaseMonster.BaseExp;
            int enemyLvl = faintedUnit.Monster.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLvl * trainerBonus) / 7);

            playerUnit.Monster.Exp += expGain;

            yield return CoroutineTypeText($"{playerUnit.Monster.BaseMonster.Name} ha ganado {expGain} puntos de experiencia.");

            yield return playerUnit.hud.SetExpSmoothly();

            //Check Level Up
            while (playerUnit.Monster.CheckForLevelUp())
            {
                playerUnit.hud.UpdateHud();
                yield return CoroutineTypeText($"{playerUnit.Monster.BaseMonster.Name} ha subido al nivel {playerUnit.Monster.Level}.");

                //Try to learn a new move
                LearnableMove newMove = playerUnit.Monster.GetLearnableMoveAtCurrentLevel();
                if (newMove != null)
                {
                    if(playerUnit.Monster.LearntMoves.Count < 4)
                    {
                        playerUnit.Monster.LearnMove(newMove);
                        yield return CoroutineTypeText($"{playerUnit.Monster.BaseMonster.Name} ha aprendido {newMove.Move.Name}.");
                        dialogBox.SetMoveNames(playerUnit.Monster.LearntMoves);
                    }
                    else
                    {
                        yield return CoroutineTypeText($"{playerUnit.Monster.BaseMonster.Name} intenta aprender {newMove.Move.Name}.");
                        yield return CoroutineTypeText($"Pero no puede aprender mas de 4 movimientos.");
                        yield return ChooseMoveToForget(playerUnit.Monster, newMove.Move);
                        yield return new WaitUntil(() => state != BattleState.MoveToForget);
                        yield return new WaitForSeconds(1f);
                    }
                }

                yield return playerUnit.hud.SetExpSmoothly(true);
            }
            yield return new WaitForSeconds(1f);
        }

        CheckForBattleOver(faintedUnit);
    }

    private void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.isPlayerUnit)
        {
            Monster nextMonster = playerParty.GetHealthyPokemon();
            if (nextMonster == null)
            {
                BattleOver(false);
            }
            else
            {
                OpenPartyScreen();
            }
        } else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                Monster nextMonster = trainerParty.GetHealthyPokemon();
                if (nextMonster != null)
                {
                    //Send out next monster
                    StartCoroutine(AboutToUse(nextMonster));
                }
                else
                    BattleOver(true);
            }
        }
    }

    private IEnumerator SendNextTrainerMonster()
    {
        state = BattleState.Busy;

        Monster nextMonster = trainerParty.GetHealthyPokemon();
        enemyUnit.Setup(nextMonster);
        yield return CoroutineTypeText($"¡{trainer.Name} ha enviado a {nextMonster.BaseMonster.Name}!");

        state = BattleState.RunningTurn;
    }

    private IEnumerator ThrowMonsterBall()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return CoroutineTypeText("¡Robar no está bien! Por eso quieres parar al Team Andorra al menos...");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return CoroutineTypeText($"¡{player.Name} ha lanzado una Monster Ball!");

        GameObject monsterBallObj = Instantiate(monsterBall, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        Vector3 originalPos = enemyUnit.transform.position;

        //Animations

        //PlayJumpAnimation
        Sequence jumpAnimation = DOTween.Sequence();
        jumpAnimation.Append(monsterBallObj.transform.DOJump(originalPos + new Vector3(0, 2, 0), 2f, 1, 1f));
        jumpAnimation.Join(monsterBallObj.transform.DOPunchRotation(new Vector3(0, 0, 255f), 1f));
        yield return jumpAnimation.WaitForCompletion();

        //PlayCaptureAnimation
        yield return enemyUnit.PlayCaptureAnimation(monsterBallObj);

        yield return monsterBallObj.transform.DOMove(originalPos - new Vector3(0, 0.8f), 0.5f).WaitForCompletion();

        enemyUnit.transform.DOMove(monsterBallObj.transform.position, 0f);

        int shakeCount = TryToCatchMonster(enemyUnit.Monster);

        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return monsterBallObj.transform.DOPunchRotation(new Vector3(0, 0, 7f), 0.8f).WaitForCompletion();
        }

        if(shakeCount == 4)
        {
            //Monster caught
            yield return CoroutineTypeText($"¡{enemyUnit.Monster.BaseMonster.Name} ha sido capturado!");
            yield return monsterBallObj.GetComponent<SpriteRenderer>().DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddMonster(enemyUnit.Monster);
            yield return CoroutineTypeText($"{enemyUnit.Monster.BaseMonster.Name} ha sido añadido a tu equipo.");

            Destroy(monsterBallObj);
            BattleOver(true);
        }
        else
        {
            //Monster not caught
            yield return new WaitForSeconds(1f);
            yield return monsterBallObj.GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if(shakeCount < 2)
                yield return CoroutineTypeText($"¡{enemyUnit.Monster.BaseMonster.Name} ha escapado!");
            else
                yield return CoroutineTypeText($"¡Ya casi estaba!");

            Destroy(monsterBallObj);
            state = BattleState.RunningTurn;
        }

    }

    private int TryToCatchMonster(Monster monster)
    {
        float modifiedCatchRate = (3 * monster.MaxHp - 2 * monster.CurrentHP) * monster.BaseMonster.CatchRate * ConditionsDB.GetStatusBonus(monster.Status) / (3 * monster.MaxHp);

        if (modifiedCatchRate >= 255)
            return 4;

        float shakeProb = 524325 / (Mathf.Sqrt(Mathf.Sqrt(255 / modifiedCatchRate)) * 8);

        int shakeCount = 0;

        while(shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65536) >= shakeProb)
                break;
            shakeCount++;
        }
        return shakeCount;
    }

    private IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return CoroutineTypeText($"¡No seas cobarde! No puedes huir de combates contra entrenadores.");
            state = BattleState.RunningTurn;
            yield break;
        }

        escapeAttempts++;
        int playerSpeed = playerUnit.Monster.Speed;
        int enemySpeed = enemyUnit.Monster.Speed;

        if(enemySpeed < playerSpeed)
        {
            yield return CoroutineTypeText("Has escapado del combate.");
            BattleOver(true);
        }
        else
        {
            float escapeProb = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            escapeProb = escapeProb % 256;

            if(UnityEngine.Random.Range(0, 256) < escapeProb)
            {
                yield return CoroutineTypeText("Has escapado del combate.");
                BattleOver(true);
            }
            else
            {
                yield return CoroutineTypeText("¡No lograste huir!");
                state = BattleState.RunningTurn;
            }

        }
    }

}