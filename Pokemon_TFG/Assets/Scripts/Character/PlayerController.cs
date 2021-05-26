using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private Vector2 input;
    public GameObject player;

    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainersView;

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

        Collider2D interactCollider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);

        if(interactCollider != null)
        {
            //Show dialog
            interactCollider.GetComponent<Interactable>()?.Interact(transform) ;
        }
    }

    private void OnMoveOver()
    {
        CheckIfInTrainersView();
        CheckForEncounters();
    }

    private void CheckForEncounters()
    {
        
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y - 0.5f);
        if (Physics2D.OverlapCircle(playerPos, 0.2f, GameLayers.i.LongGrassLayer) != null)
        {
            Collider2D[] colliders = new Collider2D[1];
            Physics2D.OverlapCollider(player.GetComponent<Collider2D>(), new ContactFilter2D().NoFilter(), colliders);

            var enteredGrass = colliders[0].gameObject;
            MapArea mapArea = enteredGrass.GetComponent<MapArea>();

            float probability = UnityEngine.Random.Range(0.0f, 100.0f);

            float veryCommon = 10 / 187.5f;
            float common = 8.5f / 187.5f;
            float semiRare = 6.75f / 187.5f;
            float rare = 3.33f / 187.5f;
            float veryRare = 1.25f / 187.5f;

            bool encountered = false;

            if (probability <= veryRare * 100)
            {
                mapArea.Rarity = 5;
                encountered = true;
            } else if(probability <= rare * 100)
            {
                mapArea.Rarity = 4;
                encountered = true;

            } else if(probability <= semiRare * 100)
            {
                mapArea.Rarity = 3;
                encountered = true;

            } else if (probability <= common * 100)
            {
                mapArea.Rarity = 2;
                encountered = true;

            } else if(probability <= veryCommon * 100)
            {
                mapArea.Rarity = 1;
                encountered = true;
            }
            if (encountered)
            {
                character.Animator.IsMoving = false;
                character.Animator.IsRunning = false;
                OnEncountered();
            }
        }
    }

    private void CheckIfInTrainersView()
    {
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
        Collider2D collider = Physics2D.OverlapCircle(playerPos, 0.2f, GameLayers.i.FovLayer);
        if (collider != null)
        {
            character.Animator.IsMoving = false;
            character.Animator.IsRunning = false;
            OnEnterTrainersView?.Invoke(collider);
        }
    }

}
