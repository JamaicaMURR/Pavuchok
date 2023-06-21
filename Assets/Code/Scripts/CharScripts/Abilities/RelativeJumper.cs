using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeJumper : CharConnected
{
    Action DoOnPrepareJump;
    Action DoOnReleaseJump;

    Action<Collision2D> DoOnCollisionStay;

    public bool IsAirJumpAvailable { get; private set; }

    //==================================================================================================================================================================
    void OnJumpAbilityEnable()
    {
        DoOnPrepareJump = PrepareTouchJump;
        DoOnReleaseJump = ReleaseTouchJump;
    }

    void OnJumpAbilityDisable()
    {
        DoOnPrepareJump = () => { };
        DoOnReleaseJump = () => { };
    }

    //==================================================================================================================================================================
    private void Awake()
    {
        ConnectToCharacter();

        Character.EPrepareJump += OnPrepareJump;
        Character.EReleaseJump += OnReleaseJump;

        Character.EJumpAbilityEnable += OnJumpAbilityEnable;
        Character.EJumpAbilityDisable += OnJumpAbilityDisable;

        DoOnPrepareJump = () => { };
        DoOnReleaseJump = () => { };

        DoOnCollisionStay = (Collision2D c) => { };
    }

    //==================================================================================================================================================================
    private void OnCollisionStay2D(Collision2D collision)
    {
        DoOnCollisionStay(collision);
    }

    //==================================================================================================================================================================
    void OnPrepareJump() => DoOnPrepareJump();

    void OnReleaseJump() => DoOnReleaseJump();

    //==================================================================================================================================================================
    void PrepareTouchJump()
    {
        DoOnCollisionStay = TouchJump;
    }

    void ReleaseTouchJump()
    {
        DoOnCollisionStay = (c) => { };
    }

    //==================================================================================================================================================================
    void TouchJump(Collision2D collision)
    {
        Vector2 force = Physics.CounterSurfaceDirection * Sets.jumpForce;

        Physics.Collider.attachedRigidbody.AddForce(force, ForceMode2D.Impulse);

        if(collision.collider.attachedRigidbody != null)
            collision.collider.attachedRigidbody.AddForceAtPosition(-force, collision.contacts[0].point, ForceMode2D.Impulse); // second Newton's law

        ReleaseTouchJump();
    }
}

