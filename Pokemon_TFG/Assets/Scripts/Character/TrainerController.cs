using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterLost;
    [SerializeField] GameObject exclamationMark;
    [SerializeField] GameObject fov;

    private Character character;
    private bool battleLost = false;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.Facing);
    }

    public void Interact(Transform initiator)
    {

        character.LookTowards(initiator.position);

        if (!battleLost)
        {
            TrainerPerformingAction = true;

            fov.GetComponent<TrainerFov>().AudioManager.ChangeMusic(fov.GetComponent<TrainerFov>().TrainerMusic);

            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
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

        exclamationMark.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamationMark.SetActive(false);

        Vector2 diff = player.transform.position - transform.position;
        Vector2 moveVector = diff - diff.normalized;
        moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));

        yield return character.Move(moveVector, false);

        player.Character.LookTowards(transform.position);

        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
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

    public void BattleLost()
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterLost));
        fov.gameObject.SetActive(false);
        battleLost = true;
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

    public bool TrainerPerformingAction { get; private set; }

    public Sprite Sprite
    {
        get { return sprite; }
    }

}
