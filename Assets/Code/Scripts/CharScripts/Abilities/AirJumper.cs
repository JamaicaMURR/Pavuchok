using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirJumper : CharConnected
{
    Action DoOnPrepareJump;
    Action DoOnReleaseJump;

    Action DoOnBecomeFly;
    Action DoOnBecomeTouch;

    //==================================================================================================================================================================
    void OnAirJumpAbilityEnable()
    {
        DoOnBecomeFly = GetReadyForJumpPreparing;
        DoOnBecomeTouch = CancelAirJump;
    }

    void OnAirJumpAbilityDisable()
    {
        DoOnBecomeFly = () => { };
        DoOnBecomeTouch = () => { };
    }

    //==================================================================================================================================================================
    private void Awake()
    {
        ConnectToCharacter();

        Character.EAirJumpAbiltyEnable += OnAirJumpAbilityEnable;
        Character.EAirJumpAbilityDisable += OnAirJumpAbilityDisable;

        Character.EPrepareJump += OnPrepareJump;
        Character.EReleaseJump += OnReleaseJump;

        Physics.EBecomeFly += OnBecomeFly;
        Physics.EBecomeTouch += OnBecomeTouch;

        DoOnPrepareJump = () => { };
        DoOnReleaseJump = () => { };

        DoOnBecomeFly = () => { };
        DoOnBecomeTouch = () => { };
    }

    //==================================================================================================================================================================
    void OnPrepareJump() => DoOnPrepareJump();

    void OnReleaseJump() => DoOnReleaseJump();

    void OnBecomeFly() => DoOnBecomeFly();

    void OnBecomeTouch() => DoOnBecomeTouch();

    //==================================================================================================================================================================
    void Jump()
    {
        Vector2 force = Vector2.up * Sets.airJumpForce;
        Physics.Collider.attachedRigidbody.AddForce(force, ForceMode2D.Impulse);

        DoOnReleaseJump = () => { };

        Character.AirJumpDone();
    }

    void JumpOnRelease()
    {
        DoOnPrepareJump = () => { };
        DoOnReleaseJump = Jump;

        Character.AirJumpReady();
    }

    void GetReadyForJumpPreparing()
    {
        DoOnPrepareJump = JumpOnRelease;
    }

    void CancelAirJump()
    {
        DoOnPrepareJump = () => { };
        DoOnReleaseJump = () => { };
    }
}
