using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    //==================================================================================================================================================================
    new public Camera camera;

    public CharController charController;

    //==================================================================================================================================================================
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
            charController.ChargeJumpBegin();

        if(Input.GetButtonUp("Jump"))
            charController.ReleaseJump();

        float horizontal = Input.GetAxis("Horizontal");

        if(horizontal < 0)
            charController.RunLeft();
        else if(horizontal > 0)
            charController.RunRight();
        else
            charController.StopRun();

        if(Input.GetButtonDown("Fire1"))
            charController.ProduceWeb(camera.ScreenToWorldPoint(Input.mousePosition));

        if(Input.GetButtonDown("Fire2"))
        {
            charController.UnStick();
        }

        float vertical = Input.GetAxis("Vertical");

        if(vertical > 0)
            charController.PullWeb();
        else if(vertical < 0)
            charController.ReleaseWeb();
        else
            charController.StopWeb();

        //<><><><><><><><> TESTING
        if(Input.GetKeyDown(KeyCode.Alpha1))
            charController.jumpChargingAvailable = true;

        if(Input.GetKeyDown(KeyCode.Alpha2))
            charController.jumpChargingAvailable = false;
    }
}
