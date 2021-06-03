using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDrive : MonoBehaviour
{
    Rigidbody2D rigidBody;

    //==================================================================================================================================================================
    public event Action OnDashed;

    //==================================================================================================================================================================
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()  //========================   TEST ONLY!!!!
    {
        if(Input.GetKeyDown(KeyCode.D))
            Dash(Vector2.right);

        if(Input.GetKeyDown(KeyCode.A))
            Dash(Vector2.left);

        if(Input.GetKeyDown(KeyCode.W))
            Dash(Vector2.up * 10);

        if(Input.GetKeyDown(KeyCode.S))
            Dash(Vector2.down);
    }

    private void FixedUpdate()  //========================   TEST ONLY!!!!
    {
        if(Input.GetKey(KeyCode.LeftArrow))
            Move(Vector2.left);

        if(Input.GetKey(KeyCode.RightArrow))
            Move(Vector2.right);
    }

    //==================================================================================================================================================================
    public void Dash(Vector2 direction)
    {
        rigidBody.AddForce(direction, ForceMode2D.Impulse);

        if(OnDashed != null)
            OnDashed();
    }

    public void Move(Vector2 direction)
    {
        rigidBody.AddForce(direction);
    }
}
