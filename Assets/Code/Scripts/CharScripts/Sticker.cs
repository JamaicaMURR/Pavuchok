using System;
using System.Collections;
using UnityEngine;

public class Sticker : MonoBehaviour
{
    ValStorage valStorage;

    new Collider2D collider;

    Action<Collision2D> DoOnCoollisionEnter;
    Action<Collision2D, float> DoOnCollision;

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
                    DoOnCoollisionEnter = (c) => { };
                    DoOnCollision = (c, f) => { };

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

        DoOnCoollisionEnter = (c) => { };
        DoOnCollision = (c, f) => { };
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DoOnCoollisionEnter(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DoOnCollision(collision, valStorage.usualStickingForce);
    }

    //==================================================================================================================================================================
    void InitialStickToTheSurface(Collision2D collision) => StickToTheSurface(collision, valStorage.initialStickingForce);

    void StickToTheSurface(Collision2D collision, float stickingForce)
    {
        if(collision.gameObject.tag != "Unstickable")
        {
            Vector2 force = CalcNormal(collision) * stickingForce;

            if(Math.Abs(Vector2.Angle(Vector2.down, force)) > valStorage.stickingAngleThreshold)
            {
                collider.attachedRigidbody.AddForce(force);

                if(collision.collider.attachedRigidbody != null)
                    collision.collider.attachedRigidbody.AddForceAtPosition(-force, collision.contacts[0].point); // second Newton's law
            }
        }
    }

    Vector2 CalcNormal(Collision2D collision)
    {
        Vector2 result = new Vector2();

        Vector2[] normals = new Vector2[collision.contacts.Length];

        for(int i = 0; i < collision.contacts.Length; i++)
        {
            normals[i] = collision.contacts[i].point - (Vector2)transform.position;
            result += normals[i];
        }

        return result.normalized;
    }
}
