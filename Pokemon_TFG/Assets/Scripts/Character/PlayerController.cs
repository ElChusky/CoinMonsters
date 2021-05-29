using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour, ISavable
{

    [SerializeField] new string name;
    [SerializeField] Sprite sprite;
    [SerializeField] AnimatedTile animatedDoor;

    private Vector2 input;
    public GameObject player;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    public void HandleUpdate() {

        if (!character.IsMoving)
        {
            character.IsRunning = false;
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                input.y = 0;
            }
            else
            {
                input.x = 0;
            }

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, true, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }

    }

    private void Interact()
    {
        Vector3 facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        Vector3 interactPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.black, 1f);

        Collider2D interactCollider = Physics2D.OverlapCircle(interactPos, character.OffsetY, GameLayers.i.InteractableLayer);

        if(interactCollider != null)
        {
            //Show dialog
            character.IsMoving = false;
            character.IsRunning = false;
            character.HandleUpdate();
            interactCollider.GetComponent<Interactable>()?.Interact(transform) ;
        }
    }

    private void OnMoveOver()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);

        foreach (Collider2D collider in colliders)
        {
            IPlayerTriggerable triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    public object CaptureState()
    {
        PlayerSavableData data = new PlayerSavableData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            monsters = GetComponent<MonsterParty>().Monsters.Select(m => m.GetSavedData()).ToList()
        };
        return data;
    }

    public void RestoreState(object state)
    {
        //Restore position
        PlayerSavableData data = (PlayerSavableData)state;
        transform.position = new Vector3(data.position[0], data.position[1]);
        //Restore Party
        GetComponent<MonsterParty>().Monsters = data.monsters.Select(s => new Monster(s)).ToList();
    }

    public string Name
    {
        get { return name; }
    }

    public Sprite Sprite
    {
        get { return sprite; }
    }

    public Character Character
    {
        get { return character; }
    }
}

[System.Serializable]
public class PlayerSavableData
{
    public float[] position;
    public List<SavableMonsterData> monsters;
}
