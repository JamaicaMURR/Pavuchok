using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeJumper : MonoBehaviour // TODO: Jump charging & direction indication
{
    float stepTime;
    float stepForce;
    float jumpForce;

    int stepsDone;

    new Collider2D collider;

    Collision2DHandler DoOnCollisionStay;

    //==================================================================================================================================================================
    [HideInInspector]
    public float jumpForceInitial, jumpForcePeak, jumpChargeTime, jumpTimeWindow;

    [HideInInspector]
    public int chargingSteps;

    //==================================================================================================================================================================
    private void Awake()
    {
        collider = GetComponent<Collider2D>();

        stepTime = jumpChargeTime / chargingSteps;
        stepForce = (jumpForcePeak - jumpForceInitial) / chargingSteps;

        DoOnCollisionStay = delegate (Collision2D c) { };
    }

    private void Start()
    {
        jumpForce = jumpForceInitial;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DoOnCollisionStay(collision);
    }

    //==================================================================================================================================================================
    public void BeginCharge()
    {
        stepsDone = 0;
        StartCoroutine(ChargeStep());
    }

    public void CancelCharge()
    {
        StopAllCoroutines();
        jumpForce = jumpForceInitial;
    }

    public void Jump()
    {
        StopAllCoroutines();
        StartCoroutine(JumpReady());
    }

    //==================================================================================================================================================================
    void ChargeNextStep()
    {
        if(stepsDone < chargingSteps)
            StartCoroutine(ChargeStep());
    }

    //==================================================================================================================================================================
    void ActualJump(Collision2D collision)
    {
        StopCoroutine(JumpReady());

        Vector2 force = -(collision.contacts[0].point - (Vector2)transform.position).normalized * jumpForce;

        collider.attachedRigidbody.AddForce(force, ForceMode2D.Impulse);

        if(collision.collider.attachedRigidbody != null)
            collision.collider.attachedRigidbody.AddForce(-force, ForceMode2D.Impulse); // second Newton's law

        jumpForce = jumpForceInitial;

        DoOnCollisionStay = delegate (Collision2D c) { };
    }

    //==================================================================================================================================================================
    IEnumerator JumpReady()
    {
        DoOnCollisionStay = ActualJump;
        yield return new WaitForSeconds(jumpTimeWindow);
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

