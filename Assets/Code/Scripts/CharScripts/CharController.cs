using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    RelativeJumper jumper;
    WheelDrive wheelDrive;

    //==================================================================================================================================================================
    public float rotationSpeed = 350;
    public float rotationTorque = 1000;
    public float brakesTorque = 5;

    public float jumpForce = 10;
    public float jumpTimeWindow = 0.1f;

    //==================================================================================================================================================================
    private void Awake()
    {
        jumper = GetComponent<RelativeJumper>();
        wheelDrive = GetComponent<WheelDrive>();
    }

    private void Start()
    {
        jumper.jumpForce = jumpForce;
        jumper.jumpTimeWindow = jumpTimeWindow;
    }

    //==================================================================================================================================================================
    public void Jump()
    {
        jumper.Jump();
    }

    public void RunLeft()
    {
        wheelDrive.BeginRotate(-rotationSpeed, rotationTorque);
    }

    public void RunRight()
    {
        wheelDrive.BeginRotate(rotationSpeed, rotationTorque);
    }

    public void StopRun()
    {
        wheelDrive.BeginRotate(0, brakesTorque);
    }
}
