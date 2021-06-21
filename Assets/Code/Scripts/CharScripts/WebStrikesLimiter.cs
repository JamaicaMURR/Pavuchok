using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebStrikesLimiter : MonoBehaviour
{
    int _strikesLimit;

    int strikesLeft;

    CharController charController;

    //==================================================================================================================================================================
    public Animator[] indicators;

    //==================================================================================================================================================================
    public int StrikesLimit
    {
        get { return _strikesLimit; }
        set
        {
            _strikesLimit = value;

            if(strikesLeft > _strikesLimit)
                strikesLeft = _strikesLimit;
        }
    }

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
        if(strikesLeft < StrikesLimit)
        {
            if(strikesLeft == 0)
                OnAtLeastOneRestored();

            strikesLeft++;

            if(strikesLeft == StrikesLimit && OnFullCharge != null)
                OnFullCharge();

            foreach(Animator a in indicators)
                a.SetInteger("WebStrikesLeft", strikesLeft);
        }
    }

    public void UseStrike()
    {
        if(strikesLeft > 0)
        {
            if(strikesLeft == StrikesLimit && OnNotFullCharge != null)
                OnNotFullCharge();

            strikesLeft--;

            if(strikesLeft == 0)
                OnFullDischarge();

            foreach(Animator a in indicators)
                a.SetInteger("WebStrikesLeft", strikesLeft);
        }
        else
            throw new Exception("Trying strike web on 0 strikes left");
    }
}
