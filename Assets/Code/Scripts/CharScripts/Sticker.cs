using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticker : MonoBehaviour
{
    new Collider2D collider;

    Collision2DHandler DoOnCoollisionEnter;
    Collision2DHandler DoOnCollision;

    //==================================================================================================================================================================
    public float initialStickingForce = 100;
    public float stickingForce = 10;

    //==================================================================================================================================================================
    public bool IsSticky
    {
        get { return DoOnCollision == StickToTheSurface; }
        set
        {
            if(value)
            {
                DoOnCoollisionEnter = InitialStickToTheSurface;
                DoOnCollision = StickToTheSurface;
            }
            else
            {
                DoOnCoollisionEnter = delegate (Collision2D c) { };
                DoOnCollision = delegate (Collision2D c) { };
            }
        }
    }

    //==================================================================================================================================================================
    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        IsSticky = true;
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
        if(collision.gameObject.tag == "Stickable")
        {
            Vector2 force = (collision.contacts[0].point - (Vector2)transform.position).normalized * initialStickingForce;

            collider.attachedRigidbody.AddForce(force);

            if(collision.collider.attachedRigidbody != null)
                collision.collider.attachedRigidbody.AddForce(-force); // second Newton's law
        }
    }

    void StickToTheSurface(Collision2D collision)
    {
        if(collision.gameObject.tag == "Stickable")
        {
            Vector2 force = (collision.contacts[0].point - (Vector2)transform.position).normalized * stickingForce;

            collider.attachedRigidbody.AddForce(force);

            if(collision.collider.attachedRigidbody != null)
                collision.collider.attachedRigidbody.AddForce(-force); // second Newton's law
        }
    }
}
