using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum GameState {MainMenu, FreeRoam, Battle, Dialog, Cutscene, Paused, Menu, ConfirmOverwrite, PartyScreen, PartyMemberOptions, MonsterSummary, ConfirmHeal }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    private TrainerController trainer;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] MonsterSummaryMenu summaryMenu;

    private GameState state;
    private GameState prevState;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PreviousScene { get; private set; }

    public GameState State { get { return state; } set { state = value; } }
    public GameState PrevState { get { return prevState; } set { prevState = value; } }

    public NurseNPC Nurse { get; set; }

    public static GameController Instance { get; private set; }

    private MenuController menu;
    private MenuController confirmOverwriteMenu;
    private MenuController monsterPartyMenu;
    public MenuController confirmHealMenu;
    private List<MenuController> partyMemberMenus;

    private int currentMember;
    private MonsterParty party;
    private PartyScreen partyScreen;
    private Fader fader;
    private AudioManager audioManager;

    public AudioClip prevMusic;
    public Vector2 LastHealPosition { get; set; }

    private void Awake()
    {
        Instance = this;

        ConditionsDB.Init();
        MonstersDB.Init();
        MoveDB.Init();

        menu = GetComponents<MenuController>().Where(m => m.Menu.name.Equals("Menu")).First();
        confirmOverwriteMenu = GetComponents<MenuController>().Where(m => m.Menu.name.Equals("ConfirmOverwriteMenu")).First();
        monsterPartyMenu = GetComponents<MenuController>().Where(m => m.Menu.name.Equals("ChangeMonstersMenu")).First();
        partyMemberMenus = GetComponents<MenuController>().Where(m => m.Menu.name.Equals("PartyMemberOptions")).ToList();
        confirmHealMenu = GetComponents<MenuController>().Where(m => m.Menu.name.Equals("ConfirmHealMenu")).First();

        partyScreen = GetComponentInParent<EssentialObjects>().GetComponentInChildren<PartyScreen>(true);

        party = playerController.GetComponent<MonsterParty>();

        fader = FindObjectOfType<Fader>();

        audioManager = FindObjectOfType<AudioManager>();

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
            {
                state = GameState.FreeRoam;
            }
        };

        menu.OnBack += () =>
        {
            state = GameState.FreeRoam;
        };

        confirmOverwriteMenu.OnBack += () =>
        {
            menu.OpenMenu();
            state = GameState.Menu;
        };

        confirmHealMenu.OnBack += () =>
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(Nurse.dialogHealCanceled, () =>
            {
                state = GameState.FreeRoam;
            }));
        };

        foreach (MenuController memberOptions in partyMemberMenus)
        {
            memberOptions.OnMenuSelected += OnMemberOptionsSelected;
            memberOptions.OnBack += () =>
            {
                state = GameState.PartyScreen;
            };
        }

        summaryMenu.OnBack += () =>
        {
            summaryMenu.gameObject.SetActive(false);
            state = GameState.PartyScreen;
            monsterPartyMenu.OpenMenu();
        };

        confirmHealMenu.OnMenuSelected += OnConfirmHealMenuSelected;
        menu.OnMenuSelected += OnMenuSelected;
        confirmOverwriteMenu.OnMenuSelected += OnConfirmOverwriteMenuSelected;
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            if (InputManager.Instance.GetKeyDown(KeybindingActions.OpenMenu))
            {
                menu.OpenMenu();
                state = GameState.Menu;
                playerController.Character.IsRunning = false;
                playerController.Character.IsMoving = false;
                playerController.Character.HandleUpdate();
            }

        } else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        } else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        } else if(state == GameState.Menu)
        {
            menu.HandleUpdate();
        } else if(state == GameState.ConfirmOverwrite)
        {
            confirmOverwriteMenu.HandleUpdate();
        }
        else if (state == GameState.ConfirmHeal)
        {
            confirmHealMenu.HandleUpdate();
        }
        else if(state == GameState.PartyScreen)
        {
            HandlePartyScreenSelection();
        } else if(state == GameState.PartyMemberOptions)
        {
            partyMemberMenus[currentMember].HandleUpdate();
        } else if(state == GameState.MonsterSummary)
        {
            summaryMenu.HandleUpdate();
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
        {
            state = prevState;
        }
    }

    private IEnumerator FadeOutStartBattle()
    {
        PauseGame(false);
        yield return fader.FadeOut(0.5f);
    }

    public void StartBattle(Monster wildMonster)
    {
        StartCoroutine(FadeOutStartBattle());
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        MonsterParty playerParty = playerController.GetComponent<MonsterParty>();

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

    public void OnEnterMotherActivator(MotherNPC mother)
    {
        state = GameState.Cutscene;
        StartCoroutine(mother.OnActivatorTriggered(playerController));
    }

    private IEnumerator EndBattleCutscene()
    {
        yield return fader.FadeIn(0f);
        PauseGame(true);
        yield return fader.FadeOut(1f);
        PauseGame(false);
    }

    void EndBattle(bool won)
    {
        audioManager.ChangeMusic(prevMusic);

        if (!won)
        {
            state = GameState.FreeRoam;
            HealingParty.HealAfterDeath(playerController);
            StartCoroutine(Teleport(LastHealPosition));
        }

        if (trainer != null)
        {
            trainer.EndBattle(won);
            trainer = null;
            StartCoroutine(EndBattleCutscene());
        } else
        {
            state = GameState.FreeRoam;
        }

        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

    }

    private IEnumerator Teleport(Vector2 newPosition)
    {

        yield return fader.InstantFadeIn();

        PauseGame(true);

        playerController.Character.SetPositionAndSnapToTile(newPosition);

        yield return fader.FadeOut(0.5f);

        PauseGame(false);

        state = GameState.FreeRoam;
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PreviousScene = CurrentScene;
        CurrentScene = currScene;
    }

    private void OnMenuSelected(int selectedItem)
    {
        prevState = state;
        if(selectedItem == 0)
        {
            //Monsters selected
            currentMember = 0;
            partyScreen.Init();
            partyScreen.SetPartyData(party.Monsters);
            menu.CloseMenu();
            monsterPartyMenu.OpenMenu();
            state = GameState.PartyScreen;
        } else if(selectedItem == 1)
        {
            //Bag selected
            menu.CloseMenu();
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
            menu.CloseMenu();
        } else if(selectedItem == 4)
        {
            //Salir
            menu.CloseMenu();
        }
     
        if (prevState == state)//Means that we havent stepped in ConfirmOverwrite
        {
            state = GameState.FreeRoam;
        }
    }

    private void OnConfirmOverwriteMenuSelected(int selectedItem)
    {
        if(selectedItem == 0)
        {
            state = GameState.FreeRoam;
            SavingSystem.i.Save("saveSlot1");
            menu.CloseMenu();
            confirmOverwriteMenu.CloseMenu();
        } else
        {
            confirmOverwriteMenu.CloseMenu();
            state = GameState.Menu;
        }
    }

    public void OnConfirmHealMenuSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            confirmHealMenu.CloseMenu();
            StartCoroutine(DialogManager.Instance.ShowDialog(Nurse.dialogHealConfirmed, () =>
            {
                Nurse.healing.HealParty(playerController);
                LastHealPosition = playerController.transform.position;
                state = GameState.FreeRoam;
            }));
        }
        else
        {
            confirmHealMenu.CloseMenu();
            StartCoroutine(DialogManager.Instance.ShowDialog(Nurse.dialogHealCanceled, () =>
            {
                state = GameState.FreeRoam;
            }));
        }
    }

    private void OnMemberOptionsSelected(int selectedItem)
    {
        if (selectedItem == 0)
        {
            state = GameState.MonsterSummary;
            partyMemberMenus[currentMember].CloseMenu();
            summaryMenu.SetData(party.Monsters[currentMember]);
            summaryMenu.gameObject.SetActive(true);
            monsterPartyMenu.CloseMenu();
            menu.CloseMenu();
        }
        else
        {
            //TODO:Cambiar de monstruo
            //state = GameState.Menu;
        }
    }

    private void HandlePartyScreenSelection()
    {
        if (InputManager.Instance.GetKeyDown(KeybindingActions.Down))
            currentMember += 2;
        else if (InputManager.Instance.GetKeyDown(KeybindingActions.Up))
            currentMember -= 2;
        else if (InputManager.Instance.GetKeyDown(KeybindingActions.Right))
            currentMember++;
        else if (InputManager.Instance.GetKeyDown(KeybindingActions.Left))
            currentMember--;

        currentMember = Mathf.Clamp(currentMember, 0, party.Monsters.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (InputManager.Instance.GetKeyDown(KeybindingActions.InteractAndConfirm))
        {
            //Highlightear el monstruo seleccionado y cambiarlo por otro
            partyMemberMenus[currentMember].OpenMenu();
            state = GameState.PartyMemberOptions;
        }
        else if (InputManager.Instance.GetKeyDown(KeybindingActions.CancelAndReturn))
        {
            monsterPartyMenu.CloseMenu();
            menu.OpenMenu();
            state = GameState.Menu;
        }
    }

}
