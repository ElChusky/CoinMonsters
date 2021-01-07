using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 3f;
    public bool isMoving = false;
    public bool isAllowedToMove = true;
    private Vector2 input;
    public List<Pokemon> heldPokemons;
    public GameObject player;

    public LayerMask buildings;
    public LayerMask longGrass;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        
        if (!isMoving && isAllowedToMove)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if(Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                input.y = 0;
            }
            else
            {
                input.x = 0;
            }

            if(input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                Vector3 targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;
                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        animator.SetBool("isMoving", isMoving);
    }
    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, walkSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if(Physics2D.OverlapCircle(targetPos, 0.1f, buildings) != null)
        {
            return false;
        }
        return true;
    }

    private void CheckForEncounters()
    {
        //To make sure that player's head wont trigger the encounter since only feet should.
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y - 0.5f);
        int rarityLvl = 0;
        if (Physics2D.OverlapCircle(playerPos, 0.2f, longGrass) != null)
        {
            Collider2D[] colliders = new Collider2D[1];
            Physics2D.OverlapCollider(player.GetComponent<Collider2D>(), new ContactFilter2D().NoFilter(), colliders);

            string enteredGrass = colliders[0].gameObject.tag;

            int[][] specificGrass;

            float probability = Random.Range(0.0f, 100.0f);

            float veryCommon = 10 / 187.5f;
            float common = 8.5f / 187.5f;
            float semiRare = 6.75f / 187.5f;
            float rare = 3.33f / 187.5f;
            float veryRare = 1.25f / 187.5f;

            if (probability <= veryRare * 100)
            {
                specificGrass = Utils.GetEncounters(colliders[0].gameObject.tag, rarityLvl);
                rarityLvl = 5;
                Debug.Log("Encountered a Very Rare Pokemon");
            } else if(probability <= rare * 100)
            {
                rarityLvl = 4;
                Debug.Log("Encountered a Rare Pokemon");
            } else if(probability <= semiRare * 100)
            {
                rarityLvl = 3;
                Debug.Log("Encountered a Semi Rare Pokemon");
            } else if (probability <= common * 100)
            {
                rarityLvl = 2;
                Debug.Log("Encountered a Common Pokemon");
            } else if(probability <= veryCommon * 100)
            {
                rarityLvl = 1;
                Debug.Log("Encountered a Very Common Pokemon");
            }
        }
    }

}
