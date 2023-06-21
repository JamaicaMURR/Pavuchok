using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : CharConnected
{
    Transform _target;

    Action DoOnFixedUpdate;

    Quaternion _rotationQuaternion = Quaternion.AngleAxis(90, Vector3.forward);
    int _side = 0;

    private void Awake()
    {
        ConnectToCharacter();

        Character.ERollLeftStarted += OnLeftRoll;
        Character.ERollRightStarted += OnRightRoll;
        Character.ELeftBreakingStarted += OnStop;
        Character.ERightBreakingStarted += OnStop;

        Character.EWebDone += OnWebDone;
        Character.EWebCutted += OnWebCut;

        DoOnFixedUpdate = () => { };
    }

    private void FixedUpdate() => DoOnFixedUpdate();

    void OnLeftRoll() => _side = 1;
    void OnRightRoll() => _side = -1;
    void OnStop() => _side = 0;

    void OnWebDone(Transform target)
    {
        _target = target;
        StartPendulum();
    }

    void OnWebCut() => StopPendulum();

    void StartPendulum() => DoOnFixedUpdate = ApplyPendulumForce;
    void StopPendulum() => DoOnFixedUpdate = () => { };

    void ApplyPendulumForce()
    {
        Vector2 force = (_rotationQuaternion * ((Vector2)_target.position - (Vector2)transform.position)).normalized * Sets.pendulumForce * _side;
        Physics.Collider.attachedRigidbody.AddForce(force, ForceMode2D.Force);
    }
}
