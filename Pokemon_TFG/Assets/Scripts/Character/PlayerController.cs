﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 7f;
    public bool isMoving = false;
    public bool isRunning = false;
    public bool isAllowedToMove = true;
    public bool dialogActive = false;
    private Vector2 input;
    public GameObject player;
    public BattleLoader battleLoader;
    public Camera worldCamera;

    public LayerMask solidObjectsLayer;
    public LayerMask longGrassLayer;
    public LayerMask interactableLayer;

    private Animator animator;

    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ConditionsDB.Init();
    }

    // Update is called once per frame
    private void Update() {

        if (!dialogActive)
        {
            if (!isMoving && isAllowedToMove)
            {
                isRunning = false;
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
                    animator.SetFloat("moveX", input.x);
                    animator.SetFloat("moveY", input.y);
                    Vector3 targetPos = transform.position;
                    targetPos.x += input.x;
                    targetPos.y += input.y;
                    if (IsWalkable(targetPos))
                    {
                        StartCoroutine(MoveTowards(targetPos));
                    }
                }
            }
            animator.SetBool("isMoving", isMoving);
            animator.SetBool("isRunning", isRunning);

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Interact();
            }
        }
        else
        {
            DialogManager dialogManager = DialogManager.Instance;
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (!dialogManager.isTyping)
                {
                    dialogManager.currentLine++;
                    if (dialogManager.currentLine < dialogManager.dialog.Lines.Count)
                    {
                        StartCoroutine(dialogManager.TypeDialog(dialogManager.dialog.Lines[dialogManager.currentLine]));
                    }
                    else
                    {
                        dialogManager.currentLine = 0;
                        dialogManager.DialogBox.SetActive(false);
                        dialogActive = false;
                    }
                }
            }
        }

    }
    IEnumerator MoveTowards(Vector3 targetPos)
    {
        isMoving = true;
        isRunning = Input.GetKey(KeyCode.X);

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            float speed = walkSpeed;
            if (isRunning)
                speed = runSpeed;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

        CheckForEncounters();
    }

    private void Interact()
    {
        Vector3 facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        Vector3 interactPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.black, 1f);

        Collider2D interactCollider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);

        if(interactCollider != null)
        {
            //Show dialog
            interactCollider.GetComponent<Interactable>()?.Interact() ;
        }
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if(Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }
        return true;
    }

    private void CheckForEncounters()
    {
        //To make sure that player's head wont trigger the encounter since only feet should.
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y - 0.5f);
        if (Physics2D.OverlapCircle(playerPos, 0.2f, longGrassLayer) != null)
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

            if (probability <= veryRare * 100)
            {
                mapArea.Rarity = 5;
                animator.SetBool("isMoving", false);
                Utils.wildMonster = mapArea.GetWildMonster();
                Utils.monsterParty = gameObject.GetComponent<MonsterParty>();
                battleLoader.LoadBattle();

            } else if(probability <= rare * 100)
            {
                mapArea.Rarity = 4;
                animator.SetBool("isMoving", false);
                Utils.wildMonster = mapArea.GetWildMonster();
                Utils.monsterParty = gameObject.GetComponent<MonsterParty>();
                battleLoader.LoadBattle();

            } else if(probability <= semiRare * 100)
            {
                mapArea.Rarity = 3;
                animator.SetBool("isMoving", false);
                Utils.wildMonster = mapArea.GetWildMonster();
                Utils.monsterParty = gameObject.GetComponent<MonsterParty>();
                battleLoader.LoadBattle();

            } else if (probability <= common * 100)
            {
                mapArea.Rarity = 2;
                animator.SetBool("isMoving", false);
                Utils.wildMonster = mapArea.GetWildMonster();
                Utils.monsterParty = gameObject.GetComponent<MonsterParty>();
                battleLoader.LoadBattle();

            } else if(probability <= veryCommon * 100)
            {
                mapArea.Rarity = 1;
                animator.SetBool("isMoving", false);
                Utils.wildMonster = mapArea.GetWildMonster();
                Utils.monsterParty = gameObject.GetComponent<MonsterParty>();
                battleLoader.LoadBattle();
            }
        }
    }

}