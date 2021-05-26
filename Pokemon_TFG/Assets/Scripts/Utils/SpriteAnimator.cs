using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRate;

    int currentFrame;
    float timer;

    public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float speed = 3)
    {
        this.frameRate = 1 / speed;
        this.spriteRenderer = spriteRenderer;
        this.frames = frames;
    }

    public void Start()
    {
        currentFrame = 0;
        timer = frameRate;
        spriteRenderer.sprite = frames[0];
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if(timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
        }
    }

    public List<Sprite> Frames { get { return frames; } }

}
