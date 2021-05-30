using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{

    [SerializeField] Dialog dialog;
    [SerializeField] List<WalkPattern> movementPatterns;
    private List<Vector2> splittedPatterns = new List<Vector2>();
    private List<float> timePatterns = new List<float>();

    private NPCState state;
    private float idleTimer = 0f;
    private int currentPattern = 0;

    private TrainerController trainer;
    private Character character;

    private void Awake()
    {
        trainer = GetComponent<TrainerController>();
        character = GetComponent<Character>();
        SplitPattern();
    }

    public void Interact(Transform initiator)
    {
        if (trainer != null)
            trainer.Interact(initiator);
        else
        {
            if (state == NPCState.Idle)
            {
                state = NPCState.Dialog;
                character.LookTowards(initiator.position);
                StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
                {
                    idleTimer = 0;
                    state = NPCState.Idle;
                }));
            }
        }
    }

    private void Update()
    {
        if(trainer != null)
        {
            currentPattern = (currentPattern + Mathf.Abs(Mathf.FloorToInt(trainer.MovedTiles))) % splittedPatterns.Count;
            trainer.MovedTiles = 0f;
        }
        if (trainer == null || (trainer != null && !trainer.TrainerPerformingAction))
        {
            if (state == NPCState.Idle)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer > timePatterns[currentPattern])
                {
                    idleTimer = 0f;
                    if (splittedPatterns.Count > 0)
                    {
                        StartCoroutine(Walk());
                    }
                }
            }

            character.HandleUpdate();
        }
    }

    private void SplitPattern()
    {
        foreach (WalkPattern pattern in movementPatterns)
        {
            timePatterns.Add(pattern.Time);
            for (int i = 0; i < Mathf.Abs(pattern.Walk.x); i++)
            {
                Vector2 move = new Vector2(pattern.Walk.x / Mathf.Abs(pattern.Walk.x), 0);
                splittedPatterns.Add(move);
                timePatterns.Add(0);
            }
            for (int i = 0; i < Mathf.Abs(pattern.Walk.y); i++)
            {
                Vector2 move = new Vector2(0, pattern.Walk.y / Mathf.Abs(pattern.Walk.y));
                splittedPatterns.Add(move);
                timePatterns.Add(0);
            }
            timePatterns.RemoveAt(timePatterns.Count - 1);
        }
    }

    private IEnumerator Walk()
    {
        state = NPCState.Walking;

        Vector3 oldPos = transform.position;

        yield return character.Move(splittedPatterns[currentPattern], false);

        if(transform.position != oldPos)
            currentPattern = (currentPattern + 1) % splittedPatterns.Count;

        state = NPCState.Idle;
    }
}

public enum NPCState { Idle, Walking, Running, Dialog}

[System.Serializable]
public class WalkPattern
{
    [SerializeField] Vector2 walk;
    [SerializeField] float time;

    public Vector2 Walk { get { return walk; } }
    public float Time { get { return time; } }

}
