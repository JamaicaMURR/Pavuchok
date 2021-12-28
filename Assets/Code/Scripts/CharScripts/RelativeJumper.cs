using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeJumper : MonoBehaviour
{
    ValStorage valStorage;

    float stepTime;
    float stepForce;
    float jumpForce;

    int stepsDone;

    new Collider2D collider;

    Action DoOnJumpCharging;
    Action<Collision2D> DoOnCollisionStay;

    //==================================================================================================================================================================
    public bool JumpChargingAbility
    {
        get { return DoOnJumpCharging == ActualJumpCharge; }
        set
        {
            if(value)
                DoOnJumpCharging = ActualJumpCharge;
            else
                DoOnJumpCharging = delegate () { };
        }
    }

    //==================================================================================================================================================================
    public event Action OnChargingComplete;
    public event Action OnChargingCancelled;

    //==================================================================================================================================================================
    private void Awake()
    {
        valStorage = GameObject.Find("Master").GetComponent<ValStorage>();

        collider = GetComponent<Collider2D>();

        stepTime = valStorage.jumpChargeTime / valStorage.chargingSteps;
        stepForce = (valStorage.jumpForcePeak - valStorage.jumpForceInitial) / valStorage.chargingSteps;

        DoOnCollisionStay = (Collision2D c) => { };

        JumpChargingAbility = false;
    }

    private void Start()
    {
        jumpForce = valStorage.jumpForceInitial;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DoOnCollisionStay(collision);
    }

    //==================================================================================================================================================================
    public void BeginCharge()
    {
        DoOnJumpCharging();
    }

    public void CancelCharge()
    {
        StopAllCoroutines();
        jumpForce = valStorage.jumpForceInitial;

        if(OnChargingCancelled != null)
            OnChargingCancelled();
    }

    public void Jump()
    {
        StopAllCoroutines();
        StartCoroutine(JumpReady());
    }

    //==================================================================================================================================================================
    void ChargeNextStep()
    {
        if(stepsDone < valStorage.chargingSteps)
            StartCoroutine(ChargeStep());
        else if(OnChargingComplete != null)
            OnChargingComplete();

    }

    //==================================================================================================================================================================
    void ActualJumpCharge()
    {
        stepsDone = 0;
        StartCoroutine(ChargeStep());
    }

    void ActualJump(Collision2D collision)
    {
        StopCoroutine(JumpReady());

        Vector2 force = -(collision.contacts[0].point - (Vector2)transform.position).normalized * jumpForce;

        collider.attachedRigidbody.AddForce(force, ForceMode2D.Impulse);

        if(collision.collider.attachedRigidbody != null)
            collision.collider.attachedRigidbody.AddForceAtPosition(-force, collision.contacts[0].point, ForceMode2D.Impulse); // second Newton's law

        jumpForce = valStorage.jumpForceInitial;

        DoOnCollisionStay = delegate (Collision2D c) { };
    }

    //==================================================================================================================================================================    
    IEnumerator JumpReady()
    {
        DoOnCollisionStay = ActualJump;
        yield return new WaitForSeconds(valStorage.jumpTimeWindow);

        jumpForce = valStorage.jumpForceInitial;

        DoOnCollisionStay = delegate (Collision2D c) { };
    }

    IEnumerator ChargeStep()
    {
        yield return new WaitForSeconds(stepTime);

        jumpForce += stepForce;
        stepsDone++;

        ChargeNextStep();
    }
}

