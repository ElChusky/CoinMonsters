using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene, Paused, Menu, ConfirmOverwrite, PartyScreen }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    private TrainerController trainer;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    public GameState state;
    GameState prevState;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PreviousScene { get; private set; }

    public static GameController Instance { get; private set; }

    private MenuController menuController;
    private MenuController confirmOverwriteMenu;
    private MenuController changeMonsterMenu;

    private int currentMember;
    private MonsterParty party;
    private PartyScreen partyScreen;

    private void Awake()
    {
        Instance = this;

        ConditionsDB.Init();
        MonstersDB.Init();
        MoveDB.Init();

        menuController = GetComponents<MenuController>().Where(m => m.Menu.name.Equals("Menu")).First();
        confirmOverwriteMenu = GetComponents<MenuController>().Where(m => m.Menu.name.Equals("ConfirmOverwrite")).First();
        changeMonsterMenu = GetComponents<MenuController>().Where(m => m.Menu.name.Equals("ChangeMonstersMenu")).First();

        partyScreen = GetComponentInParent<EssentialObjects>().GetComponentInChildren<PartyScreen>(true);

        party = playerController.GetComponent<MonsterParty>();

    }

    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };

        menuController.OnBack += () =>
        {
            state = GameState.FreeRoam;
        };

        menuController.OnMenuSelected += OnMenuSelected;
        confirmOverwriteMenu.OnMenuSelected += OnConfirmMenuSelected;
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
                playerController.Character.IsRunning = false;
                playerController.Character.IsMoving = false;
                playerController.Character.HandleUpdate();
            }

        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        } else if(state == GameState.Menu)
        {
            menuController.HandleUpdate();
        } else if(state == GameState.ConfirmOverwrite)
        {
            confirmOverwriteMenu.HandleUpdate();
        } else if(state == GameState.PartyScreen)
        {
            HandlePartyScreenSelection();
        }

    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else
            state = prevState;
    }

    public void StartBattle(MapArea mapArea)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        MonsterParty playerParty = playerController.GetComponent<MonsterParty>();
        Monster wildMonster = mapArea.GetWildMonster();

        Monster wildMonsterCopy = new Monster(wildMonster.BaseMonster, wildMonster.Level);

        battleSystem.StartBattle(playerParty, wildMonsterCopy);
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        this.trainer = trainer;

        MonsterParty playerParty = playerController.GetComponent<MonsterParty>();
        MonsterParty trainerParty = trainer.GetComponent<MonsterParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    void EndBattle(bool won)
    {
        if(trainer != null && won)
        {
            trainer.BattleLost();
            trainer = null;
        }

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PreviousScene = CurrentScene;
        CurrentScene = currScene;
    }

    private void OnMenuSelected(int selectedItem)
    {
        GameState prevState = state;
        if(selectedItem == 0)
        {
            //Monsters selected
            currentMember = 0;
            partyScreen.Init();
            partyScreen.SetPartyData(party.Monsters);
            changeMonsterMenu.OpenMenu();
            state = GameState.PartyScreen;
        } else if(selectedItem == 1)
        {
            //Bag selected
        } else if(selectedItem == 2)
        {
            //Guardar
            if(File.Exists(Path.Combine(Application.persistentDataPath, "saveSlot1")))
            {
                //Dialog asking for overwriting
                StartCoroutine(DialogManager.Instance.ShowDialog(new Dialog(new List<string>() 
                {
                    "Ya existe una partida guardada.",
                    "¿Deseas sobreescribirla?"
                }
                ), () => 
                {
                    confirmOverwriteMenu.OpenMenu();
                    state = GameState.ConfirmOverwrite;
                }));
            } else
                SavingSystem.i.Save("saveSlot1");
        } else if(selectedItem == 3)
        {
            //Cargar
            SavingSystem.i.Load("saveSlot1");
        } else if(selectedItem == 4)
        {
            //Salir
        }
        
        if(prevState == state) //Means that we havent stepped in ConfirmOverwrite
            state = GameState.FreeRoam;
    }

    private void OnConfirmMenuSelected(int selectedItem)
    {
        if(selectedItem == 0)
            SavingSystem.i.Save("saveSlot1");
        state = GameState.FreeRoam;
    }

    private void HandlePartyScreenSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            currentMember++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentMember--;

        currentMember = Mathf.Clamp(currentMember, 0, party.Monsters.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //Highlightear el monstruo seleccionado y cambiarlo por otro
            Debug.Log("Monster seleccionado");

        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            changeMonsterMenu.CloseMenu();
            state = GameState.FreeRoam;
        }
    }

}
