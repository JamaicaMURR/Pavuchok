using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    Action TestControl;

    public Character Character;

    //==================================================================================================================================================================
    private void Awake()
    {
        TestControl = () => { };
    }

    private void Start()
    {
        Character.RollAbility = true;
        Character.JumpAbility = true;
    }

    void Update()
    {

        // Rolling
        float horizontal = Input.GetAxis("Horizontal");

        if(horizontal < 0)
            Character.RollLeftCommand();
        else if(horizontal > 0)
            Character.RollRightCommand();
        else
            Character.StopCommand();

        // Jumping
        if(Input.GetButtonDown("Jump"))
            Character.PrepareJump();

        if(Input.GetButtonUp("Jump"))
            Character.ReleaseJump();

        // Sticking
        if(Input.GetButtonUp("Fire2"))
            Character.StickCommand();

        // Web producing
        if(Input.GetButtonDown("Fire1"))
            Character.ProduceWebCommand();

        // Unsticking and cut web
        if(Input.GetButtonDown("Fire2"))
        {
            Character.UnstickCommand();
            Character.CutWebCommand();
        }

        // Changing web length
        float vertical = Input.GetAxis("Vertical");

        Character.ChangeWebLengthCommand(-vertical * Character.settings.lengthChangingSpeed * Time.deltaTime);

        // Test mode
        if(Input.GetButton("Enable Debug Button 1") && Input.GetButtonDown("Enable Debug Button 2"))
        {
            Character.AirJumpAbility = true;
            Character.StickAbility = true;
            Character.WebAbility = true;
            Character.ChangeWebLengthAbility = true;
            Character.ChuteAbility = true;

            Debug.Log("TestMode On");

            TestControl = delegate ()
            {
                if(Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Character.AirJumpAbility = !Character.AirJumpAbility;

                    if(Character.AirJumpAbility)
                        Debug.Log("DoubleJumpAbility On");
                    else
                        Debug.Log("DoubleJumpAbility Off");
                }

                if(Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Character.StickAbility = !Character.StickAbility;

                    if(Character.StickAbility)
                        Debug.Log("StickAbility On");
                    else
                        Debug.Log("StickAbility Off");
                }

                if(Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Character.WebAbility = !Character.WebAbility;

                    if(Character.WebAbility)
                        Debug.Log("WebAbility On");
                    else
                        Debug.Log("WebAbility Off");
                }

                if(Input.GetKeyDown(KeyCode.Alpha4))
                {
                    Character.ChangeWebLengthAbility = !Character.ChangeWebLengthAbility;

                    if(Character.ChangeWebLengthAbility)
                        Debug.Log("ChangeWebLengthAbility On");
                    else
                        Debug.Log("ChangeWebLengthAbility Off");
                }

                if(Input.GetKeyDown(KeyCode.Alpha5))
                {
                    Character.ChuteAbility = !Character.ChuteAbility;

                    if(Character.ChuteAbility)
                        Debug.Log("ChuteAbility On");
                    else
                        Debug.Log("ChuteAbility Off");
                }

                if(Input.GetKeyDown(KeyCode.Alpha6))
                {
                    //charController.Immortality = !charController.Immortality;

                    //if(charController.Immortality)
                    //    Debug.Log("Immortality On");
                    //else
                    //    Debug.Log("Immortality Off");
                }
            };
        }

        TestControl();
    }
}
