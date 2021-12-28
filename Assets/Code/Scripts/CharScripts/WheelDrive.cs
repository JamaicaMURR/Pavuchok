using UnityEngine;

public class WheelDrive : MonoBehaviour
{
    new HingeJoint2D hingeJoint;

    //==================================================================================================================================================================
    private void Awake()
    {
        hingeJoint = GetComponent<HingeJoint2D>();
    }

    //==================================================================================================================================================================
    public void BeginRotate(float speed, float torque)
    {
        hingeJoint.motor = new JointMotor2D() { motorSpeed = speed, maxMotorTorque = torque };
    }
}

