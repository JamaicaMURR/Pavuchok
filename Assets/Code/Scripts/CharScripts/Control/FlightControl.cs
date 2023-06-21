using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightControl : CharConnected
{
    Action DoOnFixedUpdate;

    int _side;

    private void Awake()
    {
        ConnectToCharacter();

        Character.ERollLeftStarted += OnLeftRoll;
        Character.ERollRightStarted += OnRightRoll;
        Character.ELeftBreakingStarted += OnStop;
        Character.ERightBreakingStarted += OnStop;

        Character.EWebDone += OnWebDone;
        Character.EWebCutted += OnWebCut;

        Physics.EBecomeFly += OnBecomeFly;
        Physics.EBecomeTouch += OnBecomeTouch;

        //CleanDelegates();

        DoOnFixedUpdate = () => { };
    }

    private void FixedUpdate()
    {
        DoOnFixedUpdate();
    }

    void OnLeftRoll() => _side = 1;
    void OnRightRoll() => _side = -1;
    void OnStop() => _side = 0;

    void OnBecomeFly()
    {
        StartControl();
    }

    void OnBecomeTouch()
    {
        StopControl();
    }

    void OnWebDone(Transform target)
    {
        StopControl();            
    }

    void OnWebCut()
    {
        if(!Physics.IsTouching)
            StartControl();
    }

    void StartControl() => DoOnFixedUpdate = ApplyControlForce;

    void StopControl() => DoOnFixedUpdate = () => { };

    void ApplyControlForce()
    {
        Vector2 force = Vector2.left * Sets.flightControlForce * _side;
        Physics.Collider.attachedRigidbody.AddForce(force, ForceMode2D.Force);
    }
}
