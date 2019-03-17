using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerState state;

    public Vector2 ls = Vector2.zero;
    public bool xy = false;
    public bool down = false;

    private float previousLSV = 0f;

    // Start is called before the first frame update
    void Start()
    {
        state = GetComponent<PlayerState>();
        movement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        if (state.id == PlayerState.PlayerID.p1)
        {
            ls = new Vector2(Input.GetAxisRaw("1LSH"), Input.GetAxisRaw("1LSV"));
            down = ls.y == -1f;

            if (down && ls.y != previousLSV) movement.pressDown();
            previousLSV = ls.y;

            xy = Input.GetButton("1XY");
            if (Input.GetButtonDown("1XY"))movement.pressXY(dt);
        }
    }
}
