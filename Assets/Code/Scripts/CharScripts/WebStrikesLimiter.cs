using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebStrikesLimiter : MonoBehaviour
{
    int _strikesLeft;

    CharController charController;

    //==================================================================================================================================================================
    int StrikesLeft
    {
        get { return _strikesLeft; }
        set
        {
            if(value > 0)
            {
                if(_strikesLeft == 0)
                    charController.webProducingAvailable = true;

                _strikesLeft = value;
            }
            else if(value == 0)
            {
                _strikesLeft = 0;
                charController.webProducingAvailable = false;
            }
            else
                throw new System.Exception("StrikesLeft < 0");
        }
    }

    //==================================================================================================================================================================
    public int strikesLimit = 3;

    //==================================================================================================================================================================
    private void Awake()
    {
        charController = GetComponent<CharController>();

        GetComponent<WebProducer>().OnWebDone += delegate () { StrikesLeft--; };
    }

    //==================================================================================================================================================================
    public void RestoreStrike()
    {
        if(StrikesLeft < strikesLimit)
            StrikesLeft++;
    }
}
