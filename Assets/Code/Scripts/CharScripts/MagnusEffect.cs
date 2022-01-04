using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnusEffect : MonoBehaviour
{
    ValStorage valStorage;

    new Rigidbody2D rigidbody;

    CharController charController;

    Action DoOnFixedUpdate;

    private void Awake()
    {
        valStorage = GameObject.Find("Master").GetComponent<ValStorage>();

        rigidbody = GetComponent<Rigidbody2D>();
        charController = GetComponent<CharController>();

        DoOnFixedUpdate = () => { };

        charController.OnBecomeFly += () => DoOnFixedUpdate = ApplyMagnusForce;
        charController.OnBecomeTouch += () => DoOnFixedUpdate = () => { };
    }

    private void FixedUpdate() => DoOnFixedUpdate();

    void ApplyMagnusForce()
    {
        Vector2 force = Quaternion.Euler(0, 0, 90) * rigidbody.velocity * rigidbody.angularVelocity * valStorage.magnusEffectMultiplier;
        rigidbody.AddForce(force);
    }
}
