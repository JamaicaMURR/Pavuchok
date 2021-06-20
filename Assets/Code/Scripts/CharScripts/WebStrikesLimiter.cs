using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebStrikesLimiter : MonoBehaviour
{
    int strikesLeft;

    CharController charController;

    //==================================================================================================================================================================
    [HideInInspector]
    public int strikesLimit;

    public event Action OnFullCharge;
    public event Action OnNotFullCharge;
    public event Action OnFullDischarge;
    public event Action OnAtLeastOneRestored;

    //==================================================================================================================================================================
    private void Awake()
    {
        charController = GetComponent<CharController>();

        OnFullDischarge += delegate () { charController.webProducingAvailable = false; };
        OnAtLeastOneRestored += delegate () { charController.webProducingAvailable = true; };
    }

    //==================================================================================================================================================================
    public void RestoreStrike()
    {
        if(strikesLeft < strikesLimit)
        {
            if(strikesLeft == 0)
                OnAtLeastOneRestored();

            strikesLeft++;

            if(strikesLeft == strikesLimit && OnFullCharge != null)
                OnFullCharge();
        }
    }

    public void UseStrike()
    {
        if(strikesLeft > 0)
        {
            if(strikesLeft == strikesLimit && OnNotFullCharge != null)
                OnNotFullCharge();

            strikesLeft--;

            if(strikesLeft == 0)
                OnFullDischarge();
        }
        else
            throw new Exception("Trying strike web on 0 strikes left");
    }
}
