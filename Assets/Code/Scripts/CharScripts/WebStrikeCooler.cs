using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebStrikeCooler : MonoBehaviour
{
    public CharController charController;
    public Animator coolingIndicator;

    //==================================================================================================================================================================
    public void BeginCooling()
    {
        coolingIndicator.SetTrigger("BeginCool");
    }


    //==================================================================================================================================================================
    public void CoolingEnded()
    {
        charController.WebStrikeCooled();
    }
}
