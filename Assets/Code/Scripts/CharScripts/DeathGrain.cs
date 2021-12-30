using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathGrain : MonoBehaviour
{
    Action DoOnDeath;

    public event Action OnDeadlySurfaceContact;

    //==================================================================================================================================================================
    public bool Immortality
    {
        get => DoOnDeath != ActualDeath;

        set
        {
            if(value)
                DoOnDeath = () => { };
            else
                DoOnDeath = ActualDeath;
        }
    }

    //==================================================================================================================================================================
    private void Awake()
    {
        DoOnDeath = ActualDeath;

        OnDeadlySurfaceContact += () => DoOnDeath();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Deadly")
            OnDeadlySurfaceContact?.Invoke();
    }

    //==================================================================================================================================================================
    void ActualDeath()
    {
        // FIX:
        Debug.Log("Death");
    }
}
