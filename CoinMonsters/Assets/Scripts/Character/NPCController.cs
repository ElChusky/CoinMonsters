using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{

    [SerializeField] Dialog dialog;
    [SerializeField] List<WalkPattern> movementPatterns;
    public bool patternFinished;
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

    public virtual void Interact(Transform initiator)
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
        if (trainer == null)
        {
            if (state == NPCState.Idle && movementPatterns.Count != 0)
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

        }
        else if(trainer != null && !trainer.TrainerPerformingAction)
        {
            if (state == NPCState.Idle && movementPatterns.Count != 0)
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
        }
        character.HandleUpdate();
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
                timePatterns.Add(0f);
            }
            for (int i = 0; i < Mathf.Abs(pattern.Walk.y); i++)
            {
                Vector2 move = new Vector2(0, pattern.Walk.y / Mathf.Abs(pattern.Walk.y));
                splittedPatterns.Add(move);
                timePatterns.Add(0f);
            }
            timePatterns.RemoveAt(timePatterns.Count - 1);
        }
    }

    private IEnumerator Walk()
    {
        state = NPCState.Walking;

        patternFinished = false;

        Vector3 oldPos = transform.position;

        yield return character.Move(splittedPatterns[currentPattern], false);

        int nextPattern = currentPattern + 1;

        if(transform.position != oldPos)
        {
            currentPattern = nextPattern % splittedPatterns.Count;
        }

        state = NPCState.Idle;

        patternFinished = true;
    }

    public Character Character { get { return character; } set { character = value; } }

}

public enum NPCState { Idle, Walking, Running, Dialog}

[System.Serializable]
public class WalkPattern
{
    [SerializeField] Vector2 walk;
    [SerializeField] float time;

    public Vector2 Walk { get { return walk; } set { walk = value; } }
    public float Time { get { return time; } set { time = value; } }

}
