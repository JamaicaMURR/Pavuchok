using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathGrain : MonoBehaviour
{
    Action DoOnDeath;

    Resurrecter resurrecter;

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
        resurrecter = GetComponent<Resurrecter>();

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
        resurrecter.Respawn();
    }
}
