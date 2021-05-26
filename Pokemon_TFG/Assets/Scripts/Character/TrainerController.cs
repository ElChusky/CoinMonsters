using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamationMark;
    [SerializeField] GameObject fov;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.Facing);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        exclamationMark.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamationMark.SetActive(false);

        Vector2 diff = player.transform.position - transform.position;
        Vector2 moveVector = diff - diff.normalized;
        moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));

        yield return character.Move(moveVector, false);

        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
        {
            Debug.Log("Trainer Battle Started");
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
}
