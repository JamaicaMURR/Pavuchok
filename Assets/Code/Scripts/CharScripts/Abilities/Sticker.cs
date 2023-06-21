using System;
using System.Collections;
using UnityEngine;

public class Sticker : CharConnected
{
    Action<Collision2D> DoOnCollisionEnter;
    Action<Collision2D> DoOnCollisionStay;
    Action<Collision2D> DoOnCollisionExit;

    Action DoOnStick;
    Action DoOnUnstick;

    //==================================================================================================================================================================
    void OnEnableStickability()
    {
        DoOnStick = ActualStick;
        DoOnUnstick = ActualUnstick;

        OnStick();
    }

    void OnDisableStickAbility()
    {
        OnUnstick();

        DoOnStick = () => { };
        DoOnUnstick = () => { };
    }

    //==================================================================================================================================================================
    private void Awake()
    {
        ConnectToCharacter();

        Character.EStickAbilityEnable += OnEnableStickability;
        Character.EStickAbilityDisable += OnDisableStickAbility;
        Character.EStick += OnStick;
        Character.EUnstick += OnUnstick;

        CleanDelegates(); // To initialize them

        DoOnStick = () => { };
        DoOnUnstick = () => { };
    }

    //==================================================================================================================================================================
    void OnStick() => DoOnStick();

    void OnUnstick() => DoOnUnstick();

    //==================================================================================================================================================================
    void ActualStick()
    {
        DoOnCollisionEnter = InitialStickToSurface;
        DoOnCollisionStay = UsualStickToSurface;
        DoOnCollisionExit = ReturningStickToSurface;

        Character.StickingActivated();
    }

    void ActualUnstick()
    {
        CleanDelegates();

        Character.StickingDeactivated();
    }

    //==================================================================================================================================================================
    private void OnCollisionEnter2D(Collision2D collision) => DoOnCollisionEnter(collision);

    private void OnCollisionStay2D(Collision2D collision) => DoOnCollisionStay(collision);

    private void OnCollisionExit2D(Collision2D collision) => DoOnCollisionExit(collision);

    //==================================================================================================================================================================
    void InitialStickToSurface(Collision2D collision) => StickToSurface(collision, Sets.initialStickingForce);

    void UsualStickToSurface(Collision2D collision) => StickToSurface(collision, Sets.usualStickingForce);

    void ReturningStickToSurface(Collision2D collision) => StickToSurface(collision, Sets.returnStickingForce);

    void StickToSurface(Collision2D collision, float stickingForce)
    {
        if(collision.gameObject.tag != Sets.unstickableTag) // todo: find tags
        {
            Vector2 force = Physics.SurfaceDirection * stickingForce;

            if(Vector2.Angle(Vector2.down, force) > Sets.stickingAngleThreshold)
            {
                Physics.Collider.attachedRigidbody.AddForce(force);

                if(collision.collider.attachedRigidbody != null && collision.contacts.Length > 0)
                    collision.collider.attachedRigidbody.AddForceAtPosition(-force, collision.contacts[0].point); // second Newton's law
            }
        }
    }

    //==================================================================================================================================================================
    void CleanDelegates()
    {
        DoOnCollisionEnter = (c) => { };
        DoOnCollisionStay = (c) => { };
        DoOnCollisionExit = (c) => { };
    }
}
