using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelDrive : MonoBehaviour
{
    new HingeJoint2D hingeJoint;

    //==================================================================================================================================================================
    private void Awake()
    {
        hingeJoint = GetComponent<HingeJoint2D>();
    }

    private void Update()  //========================   TEST ONLY!!!!
    {
        if(Input.GetKeyDown(KeyCode.D))
            BeginRotate(350, 500);

        if(Input.GetKeyDown(KeyCode.A))
            BeginRotate(-350, 500);

        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            SetNeutral();

        if(Input.GetKeyDown(KeyCode.DownArrow))
            BrakesOn();
    }

    //==================================================================================================================================================================
    public void BeginRotate(float speed, float torque)
    {
        hingeJoint.motor = new JointMotor2D() { motorSpeed = speed, maxMotorTorque = torque };
    }

    public void SetNeutral()
    {
        BeginRotate(0, 0);
    }

    public void BrakesOn(float force = float.MaxValue)
    {
        BeginRotate(0, force);
    }

    //==================================================================================================================================================================

}
