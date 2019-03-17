using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerState playerState;
    private SpriteRenderer sprites;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerState = GetComponent<PlayerState>();
        sprites = GetComponent<SpriteRenderer>();
    }

    public void animate(PlayerState.AnimationState state)
    {
        if(playerState.animationState != state)
        {
            playerState.animationState = state;
            animator.SetTrigger(state.ToString());
        }
    }

    private void Update()
    {
        sprites.flipX = !playerState.facingRight;
    }
}
