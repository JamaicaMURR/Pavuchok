using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDrive : MonoBehaviour
{
    Rigidbody2D rigidBody;

    //==================================================================================================================================================================
    public float upImpulse = 5;
    public float downImpulse = 0;
    public float sideImpulse = 10;

    public event Action OnMoved;

    //==================================================================================================================================================================
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()  //========================   TEST ONLY!!!!
    {
        if(Input.GetKeyDown(KeyCode.D))
            Move(Direction.Right);

        if(Input.GetKeyDown(KeyCode.A))
            Move(Direction.Left);

        if(Input.GetKeyDown(KeyCode.W))
            Move(Direction.Up);

        if(Input.GetKeyDown(KeyCode.S))
            Move(Direction.Down);
    }

    //==================================================================================================================================================================
    public void Move(Direction direction)
    {
        float x = 0;
        float y = 0;

        if(direction == Direction.Up)
            y = upImpulse;
        else if(direction == Direction.Down)
            y = -downImpulse;
        else if(direction == Direction.Left)
            x = -sideImpulse;
        else
            x = sideImpulse;

        rigidBody.AddForce(new Vector2(x, y), ForceMode2D.Impulse);

        if(OnMoved != null)
            OnMoved();
    }
}
