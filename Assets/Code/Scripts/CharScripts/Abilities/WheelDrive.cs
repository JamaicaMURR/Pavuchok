using System;
using System.Collections;
using UnityEngine;

public class WheelDrive : CharConnected
{
    HingeJoint2D _hingeJoint;

    Action DoOnLeftRoll;
    Action DoOnRightRoll;

    Action DoAfterStopRoll;

    Coroutine rollCoroutine;

    enum RollingState
    {
        Stop,
        Left,
        Right
    }
    
    RollingState rollingState;

    //==================================================================================================================================================================
    private void Awake()
    {
        _hingeJoint = GetComponent<HingeJoint2D>();

        OnEnableRollAbility();

        // Character connection
        ConnectToCharacter();

        Character.ERollAbilityEnable += OnEnableRollAbility;
        Character.ERollAbilityDisable += OnDisableRollAbility;

        Character.EStartRollLeft += RollLeft;
        Character.EStartRollRight += RollRight;
        Character.EStop += StopRoll;

    }

    //==================================================================================================================================================================
    public void OnEnableRollAbility()
    {
        DoAfterStopRoll = EnableRollDelegates;
        EnableRollDelegates();
    }

    public void OnDisableRollAbility()
    {
        DoAfterStopRoll = () => { };
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

        BeginRotate(0, Sets.brakesTorque);

        if(rollingState == RollingState.Right)
            Character.RightBreakingStarted();
        else if(rollingState == RollingState.Left)
            Character.LeftBreakingStarted();

        rollingState = RollingState.Stop;

        DoAfterStopRoll();
    }

    //==================================================================================================================================================================
    void ActualLeftRoll()
    {
        rollingState = RollingState.Left;

        rollCoroutine = StartCoroutine(Roll(-Sets.initialRollingSpeed, -Sets.maximalRollingSpeed));

        Character.RollLeftStarted();

        DisableRollDelegates();
    }

    void ActualRightRoll()
    {
        rollingState = RollingState.Right;

        rollCoroutine = StartCoroutine(Roll(Sets.initialRollingSpeed, Sets.maximalRollingSpeed));

        Character.RollRightStarted();

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
        DoOnLeftRoll = () => { };
        DoOnRightRoll = () => { };
    }

    //==================================================================================================================================================================
    IEnumerator Roll(float startRollingSpeed, float targetRollingSpeed)
    {
        yield return new WaitForFixedUpdate();

        float speed = startRollingSpeed;
        float neededDelta = targetRollingSpeed - startRollingSpeed;
        float deltaPerFixedUpdate = neededDelta / Sets.accelerationTime * Time.fixedDeltaTime;

        float timeSpended = 0;

        while(timeSpended < Sets.accelerationTime)
        {
            BeginRotate(speed, Sets.rotationTorque);
            speed += deltaPerFixedUpdate;
            timeSpended += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        BeginRotate(targetRollingSpeed, Sets.rotationTorque);
    }

    //==================================================================================================================================================================
    public void BeginRotate(float speed, float torque)
    {
        _hingeJoint.motor = new JointMotor2D() { motorSpeed = speed, maxMotorTorque = torque };
    }
}

