using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    Action TestControl;

    //==================================================================================================================================================================
    new public Camera camera;

    public CharController charController;

    //==================================================================================================================================================================
    private void Awake()
    {
        TestControl = delegate () { };
    }

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

        if(Input.GetButton("Enable Debug Button 1") && Input.GetButtonDown("Enable Debug Button 2"))
        {
            charController.StickAbility = true;
            charController.JumpChargingAbility = true;
            charController.WebAbility = true;
            charController.PullReleaseAbility = true;
            charController.ChuteAbility = true;
            charController.Immortality = true;

            Debug.Log("TestMode On");

            TestControl = delegate ()
            {
                if(Input.GetKeyDown(KeyCode.Alpha1))
                {
                    charController.StickAbility = !charController.StickAbility;

                    if(charController.StickAbility)
                        Debug.Log("StickAbility On");
                    else
                        Debug.Log("StickAbility Off");
                }

                if(Input.GetKeyDown(KeyCode.Alpha2))
                {
                    charController.JumpChargingAbility = !charController.JumpChargingAbility;

                    if(charController.JumpChargingAbility)
                        Debug.Log("JumpChargingAbility On");
                    else
                        Debug.Log("JumpChargingAbility Off");
                }

                if(Input.GetKeyDown(KeyCode.Alpha3))
                {
                    charController.WebAbility = !charController.WebAbility;

                    if(charController.WebAbility)
                        Debug.Log("WebAbility On");
                    else
                        Debug.Log("WebAbility Off");
                }

                if(Input.GetKeyDown(KeyCode.Alpha4))
                {
                    charController.PullReleaseAbility = !charController.PullReleaseAbility;

                    if(charController.PullReleaseAbility)
                        Debug.Log("PullReleaseAbility On");
                    else
                        Debug.Log("PullReleaseAbility Off");
                }

                if(Input.GetKeyDown(KeyCode.Alpha5))
                {
                    charController.ChuteAbility = !charController.ChuteAbility;

                    if(charController.ChuteAbility)
                        Debug.Log("ChuteAbility On");
                    else
                        Debug.Log("ChuteAbility Off");
                }

                if(Input.GetKeyDown(KeyCode.Alpha6))
                {
                    charController.Immortality = !charController.Immortality;

                    if(charController.Immortality)
                        Debug.Log("Immortality On");
                    else
                        Debug.Log("Immortality Off");
                }
            };
        }

        TestControl();
    }
}
