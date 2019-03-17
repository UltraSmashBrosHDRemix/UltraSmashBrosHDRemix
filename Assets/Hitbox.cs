using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damage;
    public Vector2 launchDirection;
    //Minimum launch speed (simply added onto final calc)
    public float knockbackBase;
    //Extra speed (multiplied by percent)
    public float knockbackGrowth;
    //launch speed = knockbackBase + percent * knockbackGrowth 
    public bool active;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
