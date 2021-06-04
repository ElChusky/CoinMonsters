using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTrainer : TrainerController
{
    public SpecialTrainer asociatedTrainer;

    public override void Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);

        if (!BattleLost)
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

    public IEnumerator StartBattle()
    {
        if (BattleLost)
            yield break;

        GameController.Instance.prevMusic = fov.GetComponent<TrainerFov>().AudioManager.audioSource.clip;
        fov.GetComponent<TrainerFov>().AudioManager.ChangeMusic(fov.GetComponent<TrainerFov>().TrainerMusic);

        PlayerController player = FindObjectOfType<PlayerController>();
        originalPosition = transform.position;

        Vector2 moveVector = new Vector2(0, -1);

        yield return character.Move(moveVector, false, () =>
        {
            character.LookTowards(player.transform.position);
            player.Character.LookTowards(transform.position);
        });

        yield return DialogManager.Instance.ShowDialog(dialogBeforeBattle, () =>
        {
            GameController.Instance.StartTrainerBattle(this);
        });
    }

}
