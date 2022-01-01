using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnusEffect : MonoBehaviour
{
    new Rigidbody2D rigidbody;

    CharController charController;

    Action DoOnFixedUpdate;

    public float coefficient = 0.001f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        charController = GetComponent<CharController>();

        DoOnFixedUpdate = () => { };

        charController.OnBecomeFly += () => DoOnFixedUpdate = ApplyMagnusForce;
        charController.OnBecomeTouch += () => DoOnFixedUpdate = () => { };
    }

    private void FixedUpdate() => DoOnFixedUpdate();

    void ApplyMagnusForce()
    {
        Vector2 force = Quaternion.Euler(0, 0, 90) * rigidbody.velocity * rigidbody.angularVelocity * coefficient;
        rigidbody.AddForce(force);
    }
}
