using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    Action DoOnJumpRelaease;
    //==================================================================================================================================================================
    new public Camera camera;

    public CharController charController;

    //==================================================================================================================================================================
    private void Awake()
    {
        DoOnJumpRelaease = delegate () { };

    }

    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            charController.ChargeJumpBegin();
            DoOnJumpRelaease = ActualReleaseJump;
        }

        if(Input.GetButtonUp("Jump"))
            DoOnJumpRelaease();

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
            if(DoOnJumpRelaease == ActualReleaseJump)
                DoOnJumpRelaease = delegate () { }; // Jump cancelling
            else
                charController.UnStick();
        }

        float vertical = Input.GetAxis("Vertical");

        if(vertical > 0)
            charController.PullWeb();
        else if(vertical < 0)
            charController.ReleaseWeb();
        else
            charController.StopWeb();
    }

    //==================================================================================================================================================================
    void ActualReleaseJump()
    {
        charController.ReleaseJump();
    }
}
