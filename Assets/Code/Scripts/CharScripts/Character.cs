using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Knot of all character scripts
public class Character : MonoBehaviour
{
    public ValStorage valStorage;


    // Rolling ability switch events
    public event Action RollingOn;
    public event Action RollingOff;

    // Rolling commands events
    public event Action RollLeft;
    public event Action RollRight;
    public event Action Stop;

    // Rolling states events
    public event Action StartRollLeft;
    public event Action StartRollRight;
    public event Action LeftBreaking;
    public event Action RightBreaking;

    // Rolling switch
    public void RollingOnSwitch() => RollingOn?.Invoke();
    public void RollingOffSwitch() => RollingOff?.Invoke();

    // Rolling commands
    public void RollLeftCommand() => RollLeft?.Invoke();
    public void RollRightCommand() => RollRight?.Invoke();
    public void StopCommand() => Stop?.Invoke();

    // Rolling states
    public void SetStartRollLeftState() => StartRollLeft?.Invoke();
    public void SetStartRollRightState() => StartRollRight?.Invoke();
    public void SetLeftBreakingState() => LeftBreaking?.Invoke();
    public void SetRightBreakingState() => RightBreaking?.Invoke();
}
