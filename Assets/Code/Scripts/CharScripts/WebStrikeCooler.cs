using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebStrikeCooler : MonoBehaviour
{
    CharController charController;

    //==================================================================================================================================================================
    public Animator coolingIndicator;

    //==================================================================================================================================================================
    private void Awake()
    {
        charController = GetComponent<CharController>();
    }

    //==================================================================================================================================================================
    public void BeginCooling()
    {
        coolingIndicator.SetTrigger("BeginCool");
    }
}
