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
            BeginRotate(350, 1000);

        if(Input.GetKeyDown(KeyCode.A))
            BeginRotate(-350, 1000);

        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            BeginRotate(0, 5);
    }

    //==================================================================================================================================================================
    public void BeginRotate(float speed, float torque)
    {
        hingeJoint.motor = new JointMotor2D() { motorSpeed = speed, maxMotorTorque = torque };
    }
}

