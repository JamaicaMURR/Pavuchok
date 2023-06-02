using System;
using System.Collections;
using UnityEngine;

public class WheelDrive : CharConnected
{
    new HingeJoint2D hingeJoint;

    Action DoOnLeftRoll;
    Action DoOnRightRoll;

    Action DoAfterStopRoll;

    Coroutine rollCoroutine;

    [HideInInspector]
    public RollingState rollingState;

    //==================================================================================================================================================================
    private void Awake()
    {
        hingeJoint = GetComponent<HingeJoint2D>();

        EnableRollAbility();

        // Character connection
        ConnectToCharacter();

        Character.RollingOn += EnableRollAbility;
        Character.RollingOff += DisableRollAbility;

        Character.RollLeft += RollLeft;
        Character.RollRight += RollRight;
        Character.Stop += StopRoll;

    }

    //==================================================================================================================================================================
    public void EnableRollAbility()
    {
        DoAfterStopRoll = EnableRollDelegates;
        EnableRollDelegates();
    }
    public void DisableRollAbility()
    {
        DoAfterStopRoll = delegate ()
        { };
        DisableRollDelegates();
        StopRoll();
    }

    //==================================================================================================================================================================
    public void RollLeft()
    {
        DoOnLeftRoll();
    }

    public void RollRight()
    {
        DoOnRightRoll();
    }

    public void StopRoll()
    {
        if(rollCoroutine is not null)
            StopCoroutine(rollCoroutine);

        BeginRotate(0, Vals.brakesTorque);

        if(rollingState == RollingState.Right)
            Character.SetRightBreakingState();
        else if(rollingState == RollingState.Left)
            Character.SetLeftBreakingState();

        rollingState = RollingState.Stop;

        DoAfterStopRoll();
    }

    //==================================================================================================================================================================
    void ActualLeftRoll()
    {
        rollingState = RollingState.Left;

        rollCoroutine = StartCoroutine(Roll(-Vals.initialRollingSpeed, -Vals.maximalRollingSpeed));

        Character.SetStartRollLeftState();

        DisableRollDelegates();
    }

    void ActualRightRoll()
    {
        rollingState = RollingState.Right;

        rollCoroutine = StartCoroutine(Roll(Vals.initialRollingSpeed, Vals.maximalRollingSpeed));

        Character.SetStartRollRightState();

        DisableRollDelegates();
    }

    //==================================================================================================================================================================
    void EnableRollDelegates()
    {
        DoOnLeftRoll = ActualLeftRoll;
        DoOnRightRoll = ActualRightRoll;
    }

    void DisableRollDelegates()
    {
        DoOnLeftRoll = delegate ()
        { };
        DoOnRightRoll = delegate ()
        { };
    }

    //==================================================================================================================================================================
    IEnumerator Roll(float startRollingSpeed, float targetRollingSpeed)
    {
        yield return new WaitForFixedUpdate();

        float speed = startRollingSpeed;
        float neededDelta = targetRollingSpeed - startRollingSpeed;
        float deltaPerFixedUpdate = neededDelta / Vals.accelerationTime * Time.fixedDeltaTime;

        float timeSpended = 0;

        while(timeSpended < Vals.accelerationTime)
        {
            BeginRotate(speed, Vals.rotationTorque);
            speed += deltaPerFixedUpdate;
            timeSpended += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        BeginRotate(targetRollingSpeed, Vals.rotationTorque);
    }

    //==================================================================================================================================================================
    public void BeginRotate(float speed, float torque)
    {
        hingeJoint.motor = new JointMotor2D() { motorSpeed = speed, maxMotorTorque = torque };
    }
}

