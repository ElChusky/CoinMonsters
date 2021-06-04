using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;
    [SerializeField] protected Dialog dialogBeforeBattle;
    [SerializeField] protected Dialog dialogAfterLost;
    [SerializeField] GameObject exclamationMark;
    [SerializeField] protected GameObject fov;

    private protected Character character;
    private protected Vector3 originalPosition;
    private bool battleLost = false;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.Facing);
    }

    public virtual void Interact(Transform initiator)
    {

        character.LookTowards(initiator.position);

        if (!battleLost)
        {
            TrainerPerformingAction = true;

            GameController.Instance.prevMusic = fov.GetComponent<TrainerFov>().AudioManager.audioSource.clip;

            fov.GetComponent<TrainerFov>().AudioManager.ChangeMusic(fov.GetComponent<TrainerFov>().TrainerMusic);

            StartCoroutine(DialogManager.Instance.ShowDialog(dialogBeforeBattle, () =>
            {
                GameController.Instance.StartTrainerBattle(this);
            }));
        } else
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterLost));
        }
    }

    private void Update()
    {
        SetFovRotation(character.Animator.Facing);
        character.HandleUpdate();
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        TrainerPerformingAction = true;

        yield return new WaitUntil(() => GetComponent<NPCController>().patternFinished);

        originalPosition = transform.position;

        if(exclamationMark  != null)
        {
            exclamationMark.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            exclamationMark.SetActive(false);
        }

        Vector2 diff = player.transform.position - transform.position;
        Vector2 moveVector = diff - diff.normalized;
        moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));

        yield return character.Move(moveVector, false);

        player.Character.LookTowards(transform.position);

        StartCoroutine(DialogManager.Instance.ShowDialog(dialogBeforeBattle, () =>
        {
            GameController.Instance.StartTrainerBattle(this);
        }));
    }

    public void SetFovRotation(FacingDir dir)
    {
        float angle = 0f;

        if (dir == FacingDir.Right)
            angle = 90;
        else if (dir == FacingDir.Up)
            angle = 180;
        else if (dir == FacingDir.Left)
            angle = 270;
        else if (dir == FacingDir.Down)
            angle = 0;

        fov.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public void EndBattle(bool playerWon)
    {
        if (playerWon)
        {
            OnBattleLost();
        } else
        {
            transform.position = originalPosition;
            TrainerPerformingAction = false;
        }
    }

    private void OnBattleLost()
    {
        fov.gameObject.SetActive(false);
        battleLost = true;

        StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterLost, () =>
        {

            if(GetType() == typeof(SpecialTrainer))
            {
                StartCoroutine(((SpecialTrainer)this).asociatedTrainer.StartBattle());
            }

        }));
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;
        if (battleLost)
            fov.gameObject.SetActive(false);
    }

    public string Name
    {
        get { return name; }
    }

    public bool TrainerPerformingAction { get; set; }

    public bool BattleLost { get { return battleLost; } set { battleLost = value; } }

    public Sprite Sprite
    {
        get { return sprite; }
    }

}
