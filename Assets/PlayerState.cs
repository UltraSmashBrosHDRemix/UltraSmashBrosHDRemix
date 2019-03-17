using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum PlayerID {p1,p2,p3,p4};
    public enum AnimationState { stand, run, squat, jump, fall}
    public enum AirState {grounded,jump,nojump,freefall}
    public bool facingRight = true;
    
    public PlayerID id;
    public AirState airState;
    public AnimationState animationState;
    
    void Start()
    {
        airState = AirState.jump;
        animationState = AnimationState.fall;
    }
}
