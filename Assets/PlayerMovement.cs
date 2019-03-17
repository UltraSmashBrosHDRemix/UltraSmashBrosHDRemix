using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerState state;
    private PlayerInput input;
    private Rigidbody2D rb;
    private BoxCollider2D box;
    private PlayerAnimation animation;
    private PlayerAudio audio;

    private float width, height;
    private LayerMask solid, cloud;

    private Vector2 velocity = Vector2.zero;
    private float horizontalVelocity = 20f;
    private float shortHopVelocity = 7f;
    private float fullHopVelocity = 10f;
    private float doubleJumpVelocity = 10f;
    private float squatTime = 4f / 60f;
    public float gravity = -20f;
    private float normalGravity = -20f;
    private float fastFallGravity = -40f;
    public bool cloudActive = true;
    
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        state = GetComponent<PlayerState>();
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        animation = GetComponent<PlayerAnimation>();
        audio = GetComponent<PlayerAudio>();

        Bounds bounds = box.bounds;
        width = bounds.max.x - bounds.min.x;
        height = bounds.max.y - bounds.min.y;

        solid = LayerMask.GetMask("Solid");
        cloud = LayerMask.GetMask("Cloud");
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        //horizontal movement
        if (state.airState == PlayerState.AirState.grounded) run();
        else drift();

        if (velocity.x > 0.5f) state.facingRight = true;
        else if (velocity.x < -0.5f) state.facingRight = false;
        

        //vertical movement
        if (state.airState != PlayerState.AirState.grounded)
        {
            //gravity
            if (state.airState != PlayerState.AirState.grounded)
            {
                if (input.down && velocity.y <= 0f) gravity = fastFallGravity;
                velocity.y += gravity * dt;
            }
            //falling
            if (velocity.y < 0f) animation.animate(PlayerState.AnimationState.fall);
        }

        //movement corrections from terrain
        lasers(dt);

        rb.velocity = velocity;
    }

    //----INPUT PRESSES-----//
    
    public void pressXY(float dt)
    {
        if (state.airState == PlayerState.AirState.grounded)
        {
            StartCoroutine(squat());
        }
        else if(state.airState == PlayerState.AirState.jump)
        {
            doubleJump();
        }
    }

    public void pressDown()
    {
        if (state.airState == PlayerState.AirState.grounded)
        {
            drop();
        }
    }


    //-----ACTIONS-------//


    private void run()
    {
        velocity.x = input.ls.x * horizontalVelocity;

        if(Mathf.Abs(input.ls.x) > 0f)
        {
            if (state.animationState == PlayerState.AnimationState.stand) animation.animate(PlayerState.AnimationState.run);
        }
        else
        {
            if (state.animationState == PlayerState.AnimationState.run) animation.animate(PlayerState.AnimationState.stand);
        }
    }

    private void drift()
    {
        velocity.x = input.ls.x * horizontalVelocity * 0.5f;
        if (state.animationState == PlayerState.AnimationState.run)
        {
            if (velocity.y < 0f) animation.animate(PlayerState.AnimationState.fall);
            else animation.animate(PlayerState.AnimationState.jump);
        }
    }

    private IEnumerator squat()
    {
        animation.animate(PlayerState.AnimationState.squat);

        yield return new WaitForSeconds(squatTime);

        if (state.animationState == PlayerState.AnimationState.squat)
        {
            transform.position = transform.position + 0.2f * Vector3.up;
            if (input.xy) jump(fullHopVelocity);
            else jump(shortHopVelocity);
        }
    }

    private void drop()
    {
        RaycastHit2D leftSolid = Physics2D.Raycast((Vector2)box.transform.position + new Vector2(0.1f - width * 0.5f, 0.1f - height * 0.5f), Vector2.down, 0.3f, solid);
        RaycastHit2D rightSolid = Physics2D.Raycast((Vector2)box.transform.position + new Vector2(-0.1f + width * 0.5f, 0.1f - height * 0.5f), Vector2.down, 0.3f, solid);

        if (leftSolid || rightSolid)
        {
            //hit a solid so can't drop!
        }
        else
        {
            animation.animate(PlayerState.AnimationState.fall);
            transform.position = transform.position + 0.2f * Vector3.down;
        }
    }
    
    private void jump(float initialVelocity)
    {
        velocity.y = initialVelocity;
        animation.animate(PlayerState.AnimationState.jump);
        state.airState = PlayerState.AirState.jump;
        audio.Play("jump");
    }

    private void doubleJump()
    {
        velocity.y = doubleJumpVelocity;
        animation.animate(PlayerState.AnimationState.jump);
        state.airState = PlayerState.AirState.nojump;
        audio.Play("doublejump");
    }

    private void touchGround()
    {
        state.airState = PlayerState.AirState.grounded;
        animation.animate(PlayerState.AnimationState.stand);
        gravity = normalGravity;
        velocity.y = 0f;
    }

    private void fall()
    {
        state.airState = PlayerState.AirState.jump;
        animation.animate(PlayerState.AnimationState.fall);
    }

    private void lasers(float dt)
    {
        groundLasers(dt);
    }

    private void groundLasers(float dt)
    {
        bool solidHit = false;
        bool cloudHit = false;

        if (velocity.y <= 0f)
        {
            RaycastHit2D leftSolid = Physics2D.Raycast((Vector2)box.transform.position + new Vector2(0.1f - width * 0.5f, 0.1f - height * 0.5f), Vector2.down, 0.15f, solid);
            RaycastHit2D rightSolid = Physics2D.Raycast((Vector2)box.transform.position + new Vector2(-0.1f + width * 0.5f, 0.1f - height * 0.5f), Vector2.down, 0.15f, solid);

            if (leftSolid || rightSolid)
            {
                if(state.airState != PlayerState.AirState.grounded)touchGround();
                solidHit = true;
            }
        }

        cloudActive = false;
        
        if (state.airState == PlayerState.AirState.grounded)
        {
            cloudActive = true;
        }
        
        //always true if in the air and falling !!
        if (velocity.y <= 0f && !input.down)
        {
            cloudActive = true;
        }

        //if rising & holding down - land on plat
        if (state.animationState == PlayerState.AnimationState.jump && input.down)
        {
            cloudActive = true;
        }

        //if (velocity.y <= 0f && !input.down)
        if(cloudActive)
        {
            float downwardsVelocity = velocity.y > 0 ? 0f : -velocity.y * dt;

            RaycastHit2D leftCloud = Physics2D.Raycast((Vector2)box.transform.position + new Vector2(0.1f - width * 0.5f, 0.1f - height * 0.5f), Vector2.down, 0.15f + downwardsVelocity, cloud);
            RaycastHit2D rightCloud = Physics2D.Raycast((Vector2)box.transform.position + new Vector2(-0.1f + width * 0.5f, 0.1f - height * 0.5f), Vector2.down, 0.15f + downwardsVelocity, cloud);

            if (leftCloud || rightCloud)
            {
                Bounds cloudBounds;

                if (leftCloud)
                {
                    cloudBounds = leftCloud.transform.gameObject.GetComponent<EdgeCollider2D>().bounds;
                    transform.position = new Vector2(transform.position.x, cloudBounds.max.y + height * 0.5f);
                }
                else if (rightCloud)
                {
                    cloudBounds = rightCloud.transform.gameObject.GetComponent<EdgeCollider2D>().bounds;
                    transform.position = new Vector2(transform.position.x, cloudBounds.max.y + height * 0.5f);
                }

                if(state.airState != PlayerState.AirState.grounded)touchGround();
                cloudHit = true;
            }
        }

        if (!solidHit && !cloudHit && state.airState == PlayerState.AirState.grounded) fall();
        
    }
}
