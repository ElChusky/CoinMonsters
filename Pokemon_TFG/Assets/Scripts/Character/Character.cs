using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    CharacterAnimator animator;

    public float walkSpeed = 3f;
    public float runSpeed = 4.5f;

    public bool IsMoving { get; set; }
    public bool IsRunning { get; set; }
    public float OffsetY { get; private set; } = 0.3f;

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;

        transform.position = pos;
    }

    public IEnumerator Move(Vector2 moveVector, bool isPlayer, Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        Vector3 targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if (!IsPathClear(targetPos))
        {
            yield break;
        }
        IsMoving = true;

        if (isPlayer)
            IsRunning = Input.GetKey(KeyCode.X);

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            float speed = walkSpeed;
            if (animator.IsRunning)
                speed = runSpeed;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        IsMoving = false;
        IsRunning = false;

        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
        animator.IsRunning = IsRunning;
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        Vector3 diffVector = (targetPos - transform.position);
        Vector3 dirVector = diffVector.normalized;

        if(Physics2D.BoxCast(transform.position + dirVector, new Vector2(0.2f, 0.2f), 0f, dirVector, diffVector.magnitude - 1,
            GameLayers.i.SolidObjectsLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer))
        {
            return false;
        }
        return true;
    }

    public void LookTowards(Vector3 targetPos)
    {
        float xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        float ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
            Debug.LogError("Error en LookTowards: El personaje no puede mirar en diagonal.");
    }

    public CharacterAnimator Animator
    {
        get => animator;
    }
}
