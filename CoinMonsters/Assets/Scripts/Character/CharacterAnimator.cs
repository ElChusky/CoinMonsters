using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{

    [SerializeField] FacingDir facing = FacingDir.Down;

    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;

    [SerializeField] List<Sprite> runDownSprites;
    [SerializeField] List<Sprite> runUpSprites;
    [SerializeField] List<Sprite> runRightSprites;
    [SerializeField] List<Sprite> runLeftSprites;

    //Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }
    public bool IsRunning { get; set; }
    public FacingDir Facing { get { return facing; } set => facing = value; }

    //States
    //walk anims
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;
    //run anims
    SpriteAnimator runDownAnim;
    SpriteAnimator runUpAnim;
    SpriteAnimator runRightAnim;
    SpriteAnimator runLeftAnim;

    SpriteRenderer spriteRenderer;
    SpriteAnimator currentAnim;
    bool wasMoving;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        runDownAnim = new SpriteAnimator(runDownSprites, spriteRenderer, 4.5f);
        runUpAnim = new SpriteAnimator(runUpSprites, spriteRenderer, 4.5f);
        runRightAnim = new SpriteAnimator(runRightSprites, spriteRenderer, 4.5f);
        runLeftAnim = new SpriteAnimator(runLeftSprites, spriteRenderer, 4.5f);

        SetFacingDir(facing);

        currentAnim = walkDownAnim;
    }

    private void Update()
    {

        SpriteAnimator prevAnim = currentAnim;

        #region Deciding which anim depending on parameters
        if (MoveX == 1)
        {
            facing = FacingDir.Right;
            if (!IsRunning)
                currentAnim = walkRightAnim;
            else
                currentAnim = runRightAnim;
        }
        else if (MoveX == -1)
        {
            facing = FacingDir.Left;
            if (!IsRunning)
                currentAnim = walkLeftAnim;
            else
                currentAnim = runLeftAnim;
        }
        else if(MoveY == 1)
        {
            facing = FacingDir.Up;
            if (!IsRunning)
                currentAnim = walkUpAnim;
            else
                currentAnim = runUpAnim;
        } 
        else if(MoveY == -1)
        {
            facing = FacingDir.Down;
            if (!IsRunning)
                currentAnim = walkDownAnim;
            else
                currentAnim = runDownAnim;
        }
        #endregion

        if (currentAnim != prevAnim || IsMoving != wasMoving)
            currentAnim.Start();

        if (IsMoving)
        {
            currentAnim.Update();
        } else
        {
            spriteRenderer.sprite = currentAnim.Frames[0];
        }

        wasMoving = IsMoving;
    }

    public void SetFacingDir(FacingDir facingDir)
    {
        if (facingDir == FacingDir.Down)
            MoveY = -1;
        else if (facingDir == FacingDir.Up)
            MoveY = 1;
        else if (facingDir == FacingDir.Left)
            MoveX = -1;
        else if (facingDir == FacingDir.Right)
            MoveX = 1;

    }
}

public enum FacingDir { Up, Down, Left, Right}
