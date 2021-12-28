using System;
using System.Collections;
using UnityEngine;

public class Sticker : MonoBehaviour
{
    ValStorage valStorage;

    new Collider2D collider;

    Collision2DHandler DoOnCoollisionEnter;
    Collision2DHandler DoOnCollision;

    public event Action OnStickabilityEnabled;
    public event Action OnStickabilityDisabled;

    //==================================================================================================================================================================
    public bool StickAbility
    {
        get { return DoOnCollision == StickToTheSurface; }
        set
        {
            if(value != StickAbility)
            {
                if(value)
                {
                    DoOnCoollisionEnter = InitialStickToTheSurface;
                    DoOnCollision = StickToTheSurface;

                    OnStickabilityEnabled?.Invoke();
                }
                else
                {
                    DoOnCoollisionEnter = delegate (Collision2D c) { };
                    DoOnCollision = delegate (Collision2D c) { };

                    OnStickabilityDisabled?.Invoke();
                }
            }
        }
    }

    //==================================================================================================================================================================
    private void Awake()
    {
        valStorage = GameObject.Find("Master").GetComponent<ValStorage>();
        collider = GetComponent<Collider2D>();

        DoOnCoollisionEnter = delegate (Collision2D c) { };
        DoOnCollision = delegate (Collision2D c) { };
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DoOnCoollisionEnter(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DoOnCollision(collision);
    }

    //==================================================================================================================================================================
    void InitialStickToTheSurface(Collision2D collision)
    {
        if(collision.gameObject.tag != "Unstickable")
        {
            Vector2 force = (collision.contacts[0].point - (Vector2)transform.position).normalized * valStorage.initialStickingForce;

            collider.attachedRigidbody.AddForce(force);

            if(collision.collider.attachedRigidbody != null)
                collision.collider.attachedRigidbody.AddForce(-force); // second Newton's law
        }
    }

    void StickToTheSurface(Collision2D collision)
    {
        if(collision.gameObject.tag != "Unstickable")
        {
            Vector2 force = (collision.contacts[0].point - (Vector2)transform.position).normalized * valStorage.usualStickingForce;

            collider.attachedRigidbody.AddForce(force);

            if(collision.collider.attachedRigidbody != null)
                collision.collider.attachedRigidbody.AddForce(-force); // second Newton's law
        }
    }
}
