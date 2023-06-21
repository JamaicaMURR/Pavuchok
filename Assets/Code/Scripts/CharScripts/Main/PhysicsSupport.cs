using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSupport : CharConnected
{
    public Collider2D Collider;
    public Collider2D Trigger;

    Vector2 _surfDirection;
    Vector2 _counterSurfDirection;
    Vector2 _cursorPosition;
    Vector2 _mouseCursorDirection;

    bool _isCloseToSurface;
    int _triggerCollisions;

    public new Camera camera;

    // Moving states events
    public event Action EBecomeTouch;
    public event Action EBecomeFly;
    public event Action EBecomeStandStill;
    public event Action EBecomeMoving;

    public event Action ESurfaceBecomeClose;
    public event Action ESurfaceBecomeFar;

    //==================================================================================================================================================================
    void Awake()
    {
        ConnectToCharacter();

        _surfDirection = new Vector2();
        _counterSurfDirection = new Vector2();
        _cursorPosition = new Vector2();
        _mouseCursorDirection = new Vector2();
    }

    //==================================================================================================================================================================
    public Vector2 SurfaceDirection => _surfDirection;
    public Vector2 CounterSurfaceDirection => _counterSurfDirection;
    public Vector2 CursorPosition => _cursorPosition;
    public Vector2 MouseCursorDirection => _mouseCursorDirection;

    //==================================================================================================================================================================
    public bool IsTouching { get; private set; }
    public bool IsStandStill { get; private set; }

    //==================================================================================================================================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _triggerCollisions++;

        if(!_isCloseToSurface)
        {
            _isCloseToSurface = true;
            ESurfaceBecomeClose?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _triggerCollisions--;

        if(_triggerCollisions <= 0)
        {
            _isCloseToSurface = false;
            ESurfaceBecomeFar?.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) => RefreshDirections(collision);

    private void OnCollisionStay2D(Collision2D collision) => RefreshDirections(collision);

    private void FixedUpdate()
    {
        _cursorPosition = (Vector2)camera.ScreenToWorldPoint(Input.mousePosition);
        _mouseCursorDirection = (_cursorPosition - (Vector2)transform.position).normalized;
        MonitorMovingState();
    }

    //==================================================================================================================================================================
    Vector2 CalcSurfDirection(Collision2D collision)
    {
        Vector2 result = new Vector2();
        Vector2 selfPosition = (Vector2)transform.position;

        Vector2[] normals = new Vector2[collision.contacts.Length];

        for(int i = 0; i < collision.contacts.Length; i++)
        {
            normals[i] = collision.contacts[i].point - selfPosition;
            result += normals[i];
        }

        return result.normalized;
    }

    void MonitorMovingState()
    {
        if(Collider.IsTouching(new ContactFilter2D() { layerMask = LayerMask.GetMask("Default") }))
        {
            if(!IsTouching)
            {
                IsTouching = true;
                EBecomeTouch?.Invoke();
            }

            if(Collider.attachedRigidbody.velocity.magnitude <= Sets.standStillVelocityThreshold)
            {
                if(!IsStandStill)
                {
                    IsStandStill = true;
                    EBecomeStandStill?.Invoke();
                }
            }
            else
            {
                if(IsStandStill)
                {
                    IsStandStill = false;
                    EBecomeMoving?.Invoke();
                }
            }
        }
        else
        {
            if(IsTouching)
            {
                IsTouching = false;
                EBecomeFly?.Invoke();
            }
        }
    }

    void RefreshDirections(Collision2D collision)
    {
        _surfDirection = CalcSurfDirection(collision);
        _counterSurfDirection = -_surfDirection;
    }
}
