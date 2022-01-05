using System;
using System.Collections;
using UnityEngine;

public class Sticker : MonoBehaviour
{
    ValStorage valStorage;

    new Collider2D collider;

    Action<Collision2D> DoOnCollisionEnter;
    Action<Collision2D, float> DoOnCollision;
    Action<Collision2D> DoOnCollisionExit;

    Vector2 surfDirection;

    public event Action OnStickabilityEnabled;
    public event Action OnStickabilityDisabled;

    //==================================================================================================================================================================
    public Vector2 SurfaceDirection => surfDirection;

    public bool StickAbility
    {
        get { return DoOnCollision == StickToTheSurface; }
        set
        {
            if(value != StickAbility)
            {
                if(value)
                {
                    DoOnCollisionEnter = InitialStickToTheSurface;
                    DoOnCollision = StickToTheSurface;
                    DoOnCollisionExit = ReturningStickToTheSurface;

                    OnStickabilityEnabled?.Invoke();
                }
                else
                {
                    DoOnCollisionEnter = (c) => { };
                    DoOnCollision = (c, f) => { };
                    DoOnCollisionExit = (c) => { };

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

        surfDirection = new Vector2();

        DoOnCollisionEnter = (c) => { };
        DoOnCollision = (c, f) => { };
        DoOnCollisionExit = (c) => { };
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        surfDirection = CalcSurfDirection(collision);
        DoOnCollisionEnter(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        surfDirection = CalcSurfDirection(collision);
        DoOnCollision(collision, valStorage.usualStickingForce);
    }

    private void OnCollisionExit2D(Collision2D collision) => DoOnCollisionExit(collision);

    //==================================================================================================================================================================
    void InitialStickToTheSurface(Collision2D collision) => StickToTheSurface(collision, valStorage.initialStickingForce);

    void StickToTheSurface(Collision2D collision, float stickingForce)
    {
        if(collision.gameObject.tag != "Unstickable")
        {
            Vector2 force = surfDirection * stickingForce;

            if(Vector2.Angle(Vector2.down, force) > valStorage.stickingAngleThreshold)
            {
                collider.attachedRigidbody.AddForce(force);

                if(collision.collider.attachedRigidbody != null && collision.contacts.Length > 0)
                    collision.collider.attachedRigidbody.AddForceAtPosition(-force, collision.contacts[0].point); // second Newton's law
            }
        }
    }

    void ReturningStickToTheSurface(Collision2D collision) => StickToTheSurface(collision, valStorage.returnStickingForce);

    //==================================================================================================================================================================
    Vector2 CalcSurfDirection(Collision2D collision)
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
